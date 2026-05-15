using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application_Educative.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Seuil d'absences non justifiées à partir duquel une alerte est envoyée
        public const int LIMITE_ABSENCES = 3;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── PAGE NOTIFICATIONS (étudiant, prof, admin) ────────────────────────
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var role   = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var notifications = await _context.Notifications
                .Where(n => n.DestinataireRole == role && n.DestinataireId == userId)
                .OrderByDescending(n => n.DateCreation)
                .ToListAsync();

            // Si étudiant → charger absences détaillées + réclamations envoyées
            if (role == "Etudiant")
            {
                var absences = await _context.Absences
                    .Include(a => a.Matiere)
                    .Include(a => a.Section)
                    .Include(a => a.Professeur)
                    .Where(a => a.EtudiantId == userId)
                    .OrderByDescending(a => a.DateAbsence)
                    .ToListAsync();

                var reclamations = await _context.Reclamations
                    .Where(r => r.EtudiantId == userId)
                    .OrderByDescending(r => r.DateEnvoi)
                    .ToListAsync();

                int totalNonJust = absences.Count(a => !a.Justifiee);

                ViewBag.Absences         = absences;
                ViewBag.Reclamations     = reclamations;
                ViewBag.LimiteAbsences   = LIMITE_ABSENCES;
                ViewBag.DepaseLimite     = totalNonJust >= LIMITE_ABSENCES;
                ViewBag.TotalNonJust     = totalNonJust;
                ViewBag.Professeurs      = await _context.Professeurs.ToListAsync();
                ViewBag.Admins           = await _context.Admins.ToListAsync();
            }

            // Si admin ou prof → charger les réclamations reçues
            if (role == "Admin" || role == "Professeur")
            {
                var reclamationsRecues = await _context.Reclamations
                    .Include(r => r.Etudiant)
                    .Include(r => r.Absence)
                        .ThenInclude(a => a != null ? a.Matiere : null)
                    .Where(r => r.DestinataireRole == role && r.DestinataireId == userId)
                    .OrderByDescending(r => r.DateEnvoi)
                    .ToListAsync();

                ViewBag.ReclamationsRecues = reclamationsRecues;
            }

            ViewBag.Role = role;
            return View(notifications);
        }

        // ── MARQUER UNE NOTIFICATION LUE (AJAX) ──────────────────────────────
        [HttpPost]
        public async Task<IActionResult> MarquerLue(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var notif  = await _context.Notifications.FindAsync(id);

            if (notif == null || notif.DestinataireId != userId)
                return NotFound();

            notif.Lue = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ── MARQUER TOUTES LUES ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarquerToutLu()
        {
            var role   = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var nonLues = await _context.Notifications
                .Where(n => n.DestinataireRole == role && n.DestinataireId == userId && !n.Lue)
                .ToListAsync();

            nonLues.ForEach(n => n.Lue = true);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Toutes les notifications ont été marquées comme lues.";
            return RedirectToAction(nameof(Index));
        }

        // ── BADGE NAVBAR (AJAX polling) ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> NombreNonLues()
        {
            var role   = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            int count = await _context.Notifications
                .CountAsync(n => n.DestinataireRole == role && n.DestinataireId == userId && !n.Lue);

            return Json(new { count });
        }

        // ── ENVOYER UNE RÉCLAMATION ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnvoyerReclamation(
            string destinataireRole,
            int destinataireId,
            int? absenceId,
            string message)
        {
            var role      = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var etudiantId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (role != "Etudiant")
                return Forbid();

            if (string.IsNullOrWhiteSpace(message) || message.Length < 10)
            {
                TempData["ErrorMessage"] = "❌ Le message doit contenir au moins 10 caractères.";
                return RedirectToAction(nameof(Index));
            }

            var reclamation = new Reclamation
            {
                EtudiantId       = etudiantId,
                AbsenceId        = absenceId,
                DestinataireRole = destinataireRole,
                DestinataireId   = destinataireId,
                Message          = message.Trim(),
                Statut           = "En attente",
                DateEnvoi        = DateTime.Now
            };

            _context.Reclamations.Add(reclamation);
            await _context.SaveChangesAsync();

            // Notifier le destinataire
            var etudiant = await _context.Etudiants.FindAsync(etudiantId);
            string nomEtudiant = $"{etudiant?.Prenom} {etudiant?.Nom}";

            var msgTrim = message.Trim();
            var shortMsg = msgTrim.Length <= 60 ? msgTrim : msgTrim.Substring(0, 60);

            await Creer(
                _context,
                $"📩 Réclamation reçue de {nomEtudiant} : \"{shortMsg}...\"",
                destinataireRole,
                destinataireId,
                "reclamation",
                "/Notification/Index"
            );

            TempData["SuccessMessage"] = "✅ Votre réclamation a été envoyée avec succès !";
            return RedirectToAction(nameof(Index));
        }

        // ── RÉPONDRE À UNE RÉCLAMATION (admin/prof) ──────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RepondreReclamation(int reclamationId, string reponse, string statut, bool supprimerAbsence = false)
        {
            var role   = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var reclamation = await _context.Reclamations
                .Include(r => r.Etudiant)
                .FirstOrDefaultAsync(r => r.Id == reclamationId
                                       && r.DestinataireRole == role
                                       && r.DestinataireId == userId);

            if (reclamation == null) return NotFound();

            reclamation.ReponseAdmin  = reponse?.Trim();
            reclamation.DateReponse   = DateTime.Now;
            reclamation.Statut        = statut ?? "Traitée";

            if (supprimerAbsence && reclamation.AbsenceId.HasValue)
            {
                var absence = await _context.Absences.FindAsync(reclamation.AbsenceId.Value);
                if (absence != null)
                {
                    reclamation.AbsenceId = null;
                    _context.Absences.Remove(absence);
                }
            }

            await _context.SaveChangesAsync();

            // Notifier l'étudiant
            var repTrim = reponse?.Trim() ?? "";
            var shortRep = repTrim.Length <= 60 ? repTrim : repTrim.Substring(0, 60);

            await Creer(
                _context,
                $"📬 Réponse à votre réclamation : \"{shortRep}...\"",
                "Etudiant",
                reclamation.EtudiantId,
                "reclamation",
                "/Notification/Index"
            );

            TempData["SuccessMessage"] = "✅ Réponse envoyée à l'étudiant.";
            return RedirectToAction(nameof(Index));
        }

        // ── MÉTHODE STATIQUE UTILITAIRE ───────────────────────────────────────
        public static async Task Creer(
            ApplicationDbContext context,
            string message,
            string destinataireRole,
            int destinataireId,
            string type = "info",
            string? lien = null)
        {
            var notif = new Notification
            {
                Message          = message,
                DestinataireRole = destinataireRole,
                DestinataireId   = destinataireId,
                Type             = type,
                Lien             = lien,
                Lue              = false,
                DateCreation     = DateTime.Now
            };

            context.Notifications.Add(notif);
            await context.SaveChangesAsync();
        }
    }
}