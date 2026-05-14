using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application_Educative.Controllers
{
    public class AbsenceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AbsenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── INDEX ────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index(int? sectionId, int? matiereId, DateTime? date)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var query = _context.Absences
                .Include(a => a.Etudiant)
                .Include(a => a.Professeur)
                .Include(a => a.Matiere)
                .Include(a => a.Section)
                .AsQueryable();

            if (role == "Professeur")
            {
                var profId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                query = query.Where(a => a.ProfesseurId == profId);
            }
            if (role == "Etudiant")
            {
                var etudiantId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                query = query.Where(a => a.EtudiantId == etudiantId);
            }
            if (sectionId.HasValue) query = query.Where(a => a.SectionId == sectionId);
            if (matiereId.HasValue) query = query.Where(a => a.MatiereId == matiereId);
            if (date.HasValue) query = query.Where(a => a.DateAbsence.Date == date.Value.Date);

            ViewBag.Sections = await _context.Sections.ToListAsync();
            ViewBag.Matieres = await _context.Matieres.ToListAsync();
            ViewBag.SelectedSection = sectionId;
            ViewBag.SelectedMatiere = matiereId;
            ViewBag.SelectedDate = date;
            ViewBag.Role = role;

            return View(await query.OrderByDescending(a => a.DateAbsence).ToListAsync());
        }

        // ─── CREATE GET ───────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Etudiant") return RedirectToAction("Index");

            ViewBag.Matieres = await _context.Matieres.OrderBy(m => m.Nom).ToListAsync();
            ViewBag.Sections = await _context.Sections.ToListAsync();
            ViewBag.Role = role;

            if (role == "Admin")
                ViewBag.Professeurs = await _context.Professeurs.OrderBy(p => p.LastName).ToListAsync();

            return View();
        }

        // ─── AJAX : étudiants par section ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetEtudiantsBySection(int sectionId)
        {
            var section = await _context.Sections.FindAsync(sectionId);
            if (section == null) return Json(new List<object>());

            var etudiants = await _context.Etudiants
                .Where(e => e.Section == section.Nom)
                .OrderBy(e => e.Nom)
                .Select(e => new { id = e.EtudiantId, nom = e.Nom, prenom = e.Prenom })
                .ToListAsync();

            return Json(etudiants);
        }

        // ─── AJAX : étudiants par Niveau + Spécialité ────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetEtudiantsByNiveauSpecialite(string niveau, string specialite)
        {
            if (string.IsNullOrWhiteSpace(niveau) || string.IsNullOrWhiteSpace(specialite))
                return Json(new List<object>());

            var etudiants = await _context.Etudiants
                .Where(e => e.Section == niveau && e.Specialite == specialite)
                .OrderBy(e => e.Nom).ThenBy(e => e.Prenom)
                .Select(e => new { id = e.EtudiantId, nom = e.Nom, prenom = e.Prenom })
                .ToListAsync();

            return Json(etudiants);
        }

        // ─── AJAX : séances disponibles ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetSeances(int sectionId, int matiereId)
        {
            await Task.CompletedTask;
            var today = DateTime.Today;
            var seances = new[]
            {
                new { id = 1, label = today.ToString("dd/MM/yyyy") + " — 08:00",  dateStr = today.ToString("yyyy-MM-dd"), heure = "08:00" },
                new { id = 2, label = today.ToString("dd/MM/yyyy") + " — 10:00",  dateStr = today.ToString("yyyy-MM-dd"), heure = "10:00" },
                new { id = 3, label = today.AddDays(-1).ToString("dd/MM/yyyy") + " — 08:00", dateStr = today.AddDays(-1).ToString("yyyy-MM-dd"), heure = "08:00" },
                new { id = 4, label = today.AddDays(-1).ToString("dd/MM/yyyy") + " — 14:00", dateStr = today.AddDays(-1).ToString("yyyy-MM-dd"), heure = "14:00" },
                new { id = 5, label = today.AddDays(-3).ToString("dd/MM/yyyy") + " — 10:00", dateStr = today.AddDays(-3).ToString("yyyy-MM-dd"), heure = "10:00" },
            };
            return Json(seances);
        }

        // ─── ENREGISTRER ABSENCES EN MASSE ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnregistrerAbsences(
            int sectionId,
            int matiereId,
            string dateAbsence,
            string? heureSeance,
            List<int> etudiantIds,
            List<string?> motifs)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (role == "Etudiant") return Forbid();

            // Validation de base
            if (etudiantIds == null || etudiantIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Aucun étudiant sélectionné.";
                return RedirectToAction(nameof(Create));
            }

            if (!DateTime.TryParse(dateAbsence, out var dateAbs))
                dateAbs = DateTime.Today;

            int profId = role == "Professeur" ? userId : 0;

            if (role == "Admin" && Request.Form.ContainsKey("ProfesseurId"))
                int.TryParse(Request.Form["ProfesseurId"], out profId);

            var matiereAbs = await _context.Matieres.FindAsync(matiereId);

            for (int i = 0; i < etudiantIds.Count; i++)
            {
                var etudiantId = etudiantIds[i];
                var motif = i < (motifs?.Count ?? 0) ? motifs![i] : null;

                var absence = new Absence
                {
                    EtudiantId = etudiantId,
                    ProfesseurId = profId,
                    SectionId = sectionId,
                    MatiereId = matiereId,
                    DateAbsence = dateAbs,
                    Justifiee = false,
                    Commentaire = motif,
                    DateSaisie = DateTime.Now
                };

                _context.Add(absence);
                await _context.SaveChangesAsync();

                // Notification à l'étudiant
                var etudiantAbs = await _context.Etudiants.FindAsync(etudiantId);
                await NotificationController.Creer(
                    _context,
                    $"⚠️ Une absence a été enregistrée le {dateAbs:dd/MM/yyyy} en {matiereAbs?.Nom}.",
                    "Etudiant",
                    etudiantId,
                    "absence",
                    "/Notification/Index"
                );

                // Notification aux admins
                var admins = await _context.Admins.ToListAsync();
                foreach (var admin in admins)
                {
                    await NotificationController.Creer(
                        _context,
                        $"📋 Absence : {etudiantAbs?.Prenom} {etudiantAbs?.Nom} — {matiereAbs?.Nom} le {dateAbs:dd/MM/yyyy}.",
                        "Admin",
                        admin.Id,
                        "absence",
                        "/Notification/Index"
                    );
                }
            }

            TempData["SuccessMessage"] = $"✅ {etudiantIds.Count} absence(s) enregistrée(s) avec succès !";
            return RedirectToAction(nameof(Index));
        }

        // ─── TOGGLE JUSTIFICATION ─────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleJustification(int id)
        {
            var absence = await _context.Absences.FindAsync(id);
            if (absence == null) return NotFound();

            absence.Justifiee = !absence.Justifiee;
            await _context.SaveChangesAsync();

            if (absence.Justifiee)
            {
                var matiereJust = await _context.Matieres.FindAsync(absence.MatiereId);
                await NotificationController.Creer(
                    _context,
                    $"✅ Votre absence du {absence.DateAbsence:dd/MM/yyyy} en {matiereJust?.Nom} a été justifiée.",
                    "Etudiant",
                    absence.EtudiantId,
                    "justification",
                    "/Notification/Index"
                );
            }

            TempData["SuccessMessage"] = absence.Justifiee ? "✅ Absence justifiée !" : "✅ Absence marquée non justifiée !";
            return RedirectToAction(nameof(Index));
        }

        // ─── DELETE ───────────────────────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Etudiant") return Forbid();

            var absence = await _context.Absences.FindAsync(id);
            if (absence == null) return NotFound();

            _context.Absences.Remove(absence);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Absence supprimée !";
            return RedirectToAction(nameof(Index));
        }

        // ─── VUE NOTIFICATIONS ÉTUDIANT ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> MesNotifications(int? matiereId, DateTime? date, string? statut)
        {
            var etudiantId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var notifs = await _context.Notifications
                .Where(n => n.DestinataireRole == "Etudiant" && n.DestinataireId == etudiantId)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();

            var query = _context.Absences
                .Include(a => a.Matiere)
                .Include(a => a.Professeur)
                .Include(a => a.Section)
                .Where(a => a.EtudiantId == etudiantId)
                .AsQueryable();

            if (matiereId.HasValue)
                query = query.Where(a => a.MatiereId == matiereId.Value);
            if (date.HasValue)
                query = query.Where(a => a.DateAbsence.Date == date.Value.Date);
            if (statut == "justifiee")
                query = query.Where(a => a.Justifiee);
            else if (statut == "non-justifiee")
                query = query.Where(a => !a.Justifiee);

            var absences = await query.OrderByDescending(a => a.DateAbsence).ToListAsync();

            var matieresFiltres = await _context.Absences
                .Where(a => a.EtudiantId == etudiantId)
                .Select(a => a.Matiere)
                .Distinct()
                .OrderBy(m => m.Nom)
                .ToListAsync();

            ViewBag.Absences = absences;
            ViewBag.MatieresFiltres = matieresFiltres;
            ViewBag.SelectedMatiere = matiereId;
            ViewBag.SelectedDate = date;
            ViewBag.SelectedStatut = statut;

            return View(notifs);
        }

        // ─── MARQUER UNE NOTIFICATION LUE ────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarquerLue(int id)
        {
            var notif = await _context.Notifications.FindAsync(id);
            if (notif != null)
            {
                notif.Lue = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MesNotifications));
        }

        // ─── MARQUER TOUTES LES NOTIFICATIONS LUES ───────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarquerToutesLues()
        {
            var etudiantId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var notifs = await _context.Notifications
                .Where(n => n.DestinataireRole == "Etudiant" && n.DestinataireId == etudiantId && !n.Lue)
                .ToListAsync();

            notifs.ForEach(n => n.Lue = true);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MesNotifications));
        }
    }
}