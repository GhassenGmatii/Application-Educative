using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application_Educative.Controllers
{
    public class EtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EtudiantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Etudiant")
                return RedirectToAction("Login", "Admin");

            var etudiantId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.EtudiantId == etudiantId);

            if (etudiant == null)
                return RedirectToAction("Login", "Admin");

            var absences = await _context.Absences
                .Include(a => a.Matiere)
                .Include(a => a.Section)
                .Include(a => a.Professeur)
                .Where(a => a.EtudiantId == etudiantId)
                .OrderByDescending(a => a.DateAbsence)
                .ToListAsync();

            ViewBag.Etudiant = etudiant;
            ViewBag.TotalAbsences = absences.Count;
            ViewBag.Justifiees = absences.Count(a => a.Justifiee);
            ViewBag.NonJustifiees = absences.Count(a => !a.Justifiee);
            ViewBag.Absences = absences;

            return View("~/Views/GestionEtudiant/Dashboard.cshtml");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/GestionEtudiant/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                etudiant.DateCreation = DateTime.Now;
                _context.Etudiants.Add(etudiant);
                await _context.SaveChangesAsync();
                TempData["Success"] = "L'étudiant a été enregistré avec succès.";
                return RedirectToAction("Create");
            }
            return View("~/Views/GestionEtudiant/Create.cshtml", etudiant);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var etudiants = await _context.Etudiants
                .OrderBy(e => e.Nom)
                .ToListAsync();
            return View("~/Views/GestionEtudiant/Index.cshtml", etudiants);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null) return NotFound();
            return View("~/Views/GestionEtudiant/Edit.cshtml", etudiant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                etudiant.DateModification = DateTime.Now;
                _context.Etudiants.Update(etudiant);
                await _context.SaveChangesAsync();
                TempData["Success"] = "L'étudiant a été modifié avec succès.";
                return RedirectToAction("Index");
            }
            return View("~/Views/GestionEtudiant/Edit.cshtml", etudiant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant != null)
            {
                _context.Etudiants.Remove(etudiant);
                await _context.SaveChangesAsync();
                TempData["Success"] = "L'étudiant a été supprimé.";
            }
            return RedirectToAction("Index");
        }
    }
}