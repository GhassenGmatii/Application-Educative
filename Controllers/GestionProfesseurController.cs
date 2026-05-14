using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application_Educative.Controllers
{
    public class GestionProfesseurController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GestionProfesseurController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var profs = await _context.Professeurs
                .Include(p => p.ProfesseurSections)
                .ThenInclude(ps => ps.Section)
                .ToListAsync();
            return View(profs);
        }










        [HttpGet]
        public IActionResult Create()
        {
            return View(new Professeur());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Professeur prof)
        {
            var exist = await _context.Professeurs
                .FirstOrDefaultAsync(p => p.Email == prof.Email);
            if (exist != null)
                ModelState.AddModelError("Email", "❌ Cet email est déjà utilisé !");

            if (string.IsNullOrEmpty(prof.Password))
                ModelState.AddModelError("Password", "❌ Le mot de passe est obligatoire !");

            if (ModelState.IsValid)
            {
                try
                {
                    prof.Password = BCrypt.Net.BCrypt.HashPassword(prof.Password);
                    prof.DateAjout = DateTime.Now;

                    _context.Professeurs.Add(prof);
                    await _context.SaveChangesAsync();

                    // ✅ Notification de bienvenue au prof
                    await NotificationController.Creer(
                        _context,
                        $"🎉 Bienvenue {prof.FirstName} {prof.LastName} ! Votre compte professeur a été créé.",
                        "Professeur", prof.Id, "bienvenue", "/Professeur/Dashboard"
                    );

                    // ✅ Notification à l'admin
                    var admins = await _context.Admins.ToListAsync();
                    foreach (var admin in admins)
                    {
                        await NotificationController.Creer(
                            _context,
                            $"👨‍🏫 Nouveau professeur ajouté : {prof.FirstName} {prof.LastName}.",
                            "Admin", admin.Id, "info", "/GestionProfesseur/Index"
                        );
                    }

                    TempData["SuccessMessage"] = $"✅ Professeur {prof.FirstName} {prof.LastName} ajouté avec succès !";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur : " + ex.Message);
                }
            }
            return View(prof);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var prof = await _context.Professeurs.FindAsync(id);
            if (prof == null) return NotFound();
            return View(prof);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Professeur prof)
        {
            if (id != prof.Id) return NotFound();

            try
            {
                var toUpdate = await _context.Professeurs.FindAsync(id);
                if (toUpdate == null) return NotFound();

                var emailExist = await _context.Professeurs
                    .FirstOrDefaultAsync(p => p.Email == prof.Email && p.Id != id);
                if (emailExist != null)
                {
                    TempData["ErrorMessage"] = "❌ Cet email est déjà utilisé !";
                    return View(toUpdate);
                }

                toUpdate.FirstName = prof.FirstName;
                toUpdate.LastName = prof.LastName;
                toUpdate.Email = prof.Email;
                toUpdate.Telephone = prof.Telephone;
                toUpdate.Specialite = prof.Specialite;

                if (!string.IsNullOrEmpty(prof.Password))
                    toUpdate.Password = BCrypt.Net.BCrypt.HashPassword(prof.Password);

                toUpdate.DateModification = DateTime.Now;
                await _context.SaveChangesAsync();

                // ✅ Notif au prof
                await NotificationController.Creer(
                    _context,
                    $"✏️ Vos informations ont été mises à jour par l'administrateur.",
                    "Professeur", id, "info", "/Professeur/Dashboard"
                );

                TempData["SuccessMessage"] = "✅ Professeur modifié avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur : " + ex.Message);
                return View(prof);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var prof = await _context.Professeurs
                .Include(p => p.ProfesseurSections)
                .ThenInclude(ps => ps.Section)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prof == null) return NotFound();
            return View(prof);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prof = await _context.Professeurs.FindAsync(id);
            if (prof == null) return NotFound();

            _context.Professeurs.Remove(prof);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Professeur supprimé avec succès !";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Affecter(int? id)
        {
            if (id == null) return NotFound();
            var prof = await _context.Professeurs
                .Include(p => p.ProfesseurSections)
                .ThenInclude(ps => ps.Section)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prof == null) return NotFound();

            ViewBag.Sections = await _context.Sections.ToListAsync();
            return View(prof);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Affecter(int profId,
            List<int> SectionIds,
            List<int> NombreHeures,
            List<string> Jours,
            List<string> HeuresDebut,
            List<string> HeuresFin,
            List<string> DateDebuts)
        {
            var old = _context.ProfesseurSections
                .Where(ps => ps.ProfesseurId == profId);
            _context.ProfesseurSections.RemoveRange(old);

            for (int i = 0; i < SectionIds.Count; i++)
            {
                if (SectionIds[i] == 0) continue;
                var ps = new ProfesseurSection
                {
                    ProfesseurId = profId,
                    SectionId = SectionIds[i],
                    NombreHeures = i < NombreHeures.Count ? NombreHeures[i] : 0,
                    Jour = i < Jours.Count ? Jours[i] : null,
                    HeureDebut = i < HeuresDebut.Count && !string.IsNullOrEmpty(HeuresDebut[i])
                                    ? TimeSpan.Parse(HeuresDebut[i]) : null,
                    HeureFin = i < HeuresFin.Count && !string.IsNullOrEmpty(HeuresFin[i])
                                    ? TimeSpan.Parse(HeuresFin[i]) : null,
                    DateDebut = i < DateDebuts.Count && !string.IsNullOrEmpty(DateDebuts[i])
                                    ? DateTime.Parse(DateDebuts[i]) : null
                };
                _context.ProfesseurSections.Add(ps);
            }

            await _context.SaveChangesAsync();

            // ✅ Notif au prof
            await NotificationController.Creer(
                _context,
                $"📚 Vos sections ont été mises à jour par l'administrateur.",
                "Professeur", profId, "info", "/Professeur/Dashboard"
            );

            TempData["SuccessMessage"] = "✅ Sections affectées avec succès !";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string terme)
        {
            var profs = await _context.Professeurs
                .Include(p => p.ProfesseurSections)
                .ThenInclude(ps => ps.Section)
                .ToListAsync();

            if (!string.IsNullOrEmpty(terme))
            {
                profs = profs.Where(p =>
                    p.FirstName.Contains(terme, StringComparison.OrdinalIgnoreCase) ||
                    p.LastName.Contains(terme, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(terme, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                ViewBag.SearchTerm = terme;
                ViewBag.SearchPerformed = true;
            }
            return View("Index", profs);
        }
    }
}