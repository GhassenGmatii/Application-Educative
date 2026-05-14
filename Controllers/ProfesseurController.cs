using Application_Educative.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application_Educative.Controllers
{
    public class ProfesseurController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfesseurController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Professeur")
                return RedirectToAction("Login", "Admin");

            var profId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var prof = await _context.Professeurs
                .Include(p => p.ProfesseurSections)
                    .ThenInclude(ps => ps.Section)
                .FirstOrDefaultAsync(p => p.Id == profId);

            if (prof == null)
                return RedirectToAction("Login", "Admin");

            var absences = await _context.Absences
                .Include(a => a.Etudiant)
                .Include(a => a.Matiere)
                .Include(a => a.Section)
                .Where(a => a.ProfesseurId == profId)
                .OrderByDescending(a => a.DateAbsence)
                .Take(10)
                .ToListAsync();

            ViewBag.Prof = prof;
            ViewBag.TotalAbsences = await _context.Absences
                .CountAsync(a => a.ProfesseurId == profId);
            ViewBag.AbsencesNonJustifiees = await _context.Absences
                .CountAsync(a => a.ProfesseurId == profId && !a.Justifiee);
            ViewBag.RecentAbsences = absences;

            // ✅ Pointe vers le bon dossier
            return View("~/Views/GestionProfesseur/Dashboard.cshtml");
        }
    }
}