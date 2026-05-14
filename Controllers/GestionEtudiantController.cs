using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application_Educative.Controllers
{
    public class GestionEtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GestionEtudiantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var etudiants = await _context.Etudiants
                .AsNoTracking()
                .OrderBy(e => e.Nom)
                .ToListAsync();
            return View(etudiants);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Etudiant());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Etudiant etudiant, string ConfirmPassword)
        {
            if (!string.IsNullOrWhiteSpace(etudiant.Email))
            {
                var exist = await _context.Etudiants
                    .AnyAsync(e => e.Email == etudiant.Email);
                if (exist)
                    ModelState.AddModelError("Email", "❌ Cet email est déjà utilisé !");
            }

            if (string.IsNullOrWhiteSpace(etudiant.MotDePasse))
                ModelState.AddModelError("MotDePasse", "❌ Mot de passe obligatoire !");
            else if (etudiant.MotDePasse.Length < 6)
                ModelState.AddModelError("MotDePasse", "❌ Minimum 6 caractères !");

            if (etudiant.MotDePasse != ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "❌ Les mots de passe ne correspondent pas !");

            if (!ModelState.IsValid)
                return View(etudiant);

            try
            {
                etudiant.MotDePasse = BCrypt.Net.BCrypt.HashPassword(etudiant.MotDePasse);
                etudiant.DateCreation = DateTime.UtcNow;

                _context.Etudiants.Add(etudiant);
                await _context.SaveChangesAsync();

                await NotificationController.Creer(
                    _context,
                    $"🎉 Bienvenue {etudiant.Prenom} {etudiant.Nom} ! Votre compte étudiant a été créé.",
                    "Etudiant", etudiant.EtudiantId, "bienvenue", "/Etudiant/Dashboard"
                );

                var admins = await _context.Admins.ToListAsync();
                foreach (var admin in admins)
                {
                    await NotificationController.Creer(
                        _context,
                        $"🎓 Nouvel étudiant inscrit : {etudiant.Prenom} {etudiant.Nom} — {etudiant.Section}.",
                        "Admin", admin.Id, "info", "/GestionEtudiant/Index"
                    );
                }

                TempData["SuccessMessage"] = $"✅ Étudiant {etudiant.Prenom} {etudiant.Nom} ajouté avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.InnerException?.Message
                       ?? ex.InnerException?.Message
                       ?? ex.Message;
                ModelState.AddModelError("", "❌ BD : " + msg);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "❌ Erreur : " + msg);
            }

            return View(etudiant);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null) return NotFound();
            return View(etudiant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Etudiant etudiant, string? nouveauMotDePasse)
        {
            if (id != etudiant.EtudiantId) return BadRequest();

            var toUpdate = await _context.Etudiants.FindAsync(id);
            if (toUpdate == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(etudiant.Email))
            {
                var emailExist = await _context.Etudiants
                    .AnyAsync(e => e.Email == etudiant.Email && e.EtudiantId != id);
                if (emailExist)
                {
                    ModelState.AddModelError("Email", "❌ Cet email est déjà utilisé !");
                    return View(etudiant);
                }
            }

            if (!ModelState.IsValid) return View(etudiant);

            try
            {
                toUpdate.Nom = etudiant.Nom;
                toUpdate.Prenom = etudiant.Prenom;
                toUpdate.Email = etudiant.Email;
                toUpdate.Etat = etudiant.Etat;
                toUpdate.Section = etudiant.Section;
                toUpdate.Specialite = etudiant.Specialite;
                toUpdate.DateModification = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(nouveauMotDePasse))
                {
                    if (nouveauMotDePasse.Length < 6)
                    {
                        ModelState.AddModelError("nouveauMotDePasse", "❌ Minimum 6 caractères !");
                        return View(etudiant);
                    }
                    toUpdate.MotDePasse = BCrypt.Net.BCrypt.HashPassword(nouveauMotDePasse);
                }

                await _context.SaveChangesAsync();

                await NotificationController.Creer(
                    _context,
                    $"✏️ Vos informations ont été mises à jour par l'administrateur.",
                    "Etudiant", id, "info", "/Etudiant/Dashboard"
                );

                TempData["SuccessMessage"] = "✅ Étudiant modifié avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException) { return NotFound(); }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "❌ Erreur modification : " + msg);
                return View(etudiant);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null)
            {
                TempData["ErrorMessage"] = "❌ Étudiant introuvable.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Etudiants.Remove(etudiant);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Étudiant supprimé avec succès !";
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                TempData["ErrorMessage"] = "❌ Impossible de supprimer : " + msg;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string terme)
        {
            var query = _context.Etudiants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(terme))
            {
                query = query.Where(e =>
                    e.Nom.Contains(terme) ||
                    e.Prenom.Contains(terme) ||
                    e.Email.Contains(terme));
                ViewBag.SearchTerm = terme;
                ViewBag.SearchPerformed = true;
            }

            return View("Index", await query.AsNoTracking().ToListAsync());
        }
    }
}