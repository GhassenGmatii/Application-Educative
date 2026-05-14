using Application_Educative.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Professeur")
            return RedirectToAction("Dashboard", "Professeur");
        if (role == "Etudiant")
            return RedirectToAction("Dashboard", "Etudiant");

        // ── STATISTIQUES GLOBALES ─────────────────────────────────────
        ViewBag.NbEtudiants   = await _context.Etudiants.CountAsync();
        ViewBag.NbProfesseurs = await _context.Professeurs.CountAsync();
        ViewBag.NbAdmins      = await _context.Admins.CountAsync();
        ViewBag.NbMatieres    = await _context.Matieres.CountAsync();
        ViewBag.NbSections    = await _context.Sections.CountAsync();

        // ── STATISTIQUES ABSENCES ─────────────────────────────────────
        ViewBag.NbAbsencesTotal      = await _context.Absences.CountAsync();
        ViewBag.NbAbsencesJustifiees = await _context.Absences.CountAsync(a => a.Justifiee);
        ViewBag.NbAbsencesNonJust    = await _context.Absences.CountAsync(a => !a.Justifiee);

        // ── RÉCLAMATIONS ──────────────────────────────────────────────
        ViewBag.NbReclamations       = await _context.Reclamations.CountAsync();
        ViewBag.NbReclamationsAttente = await _context.Reclamations.CountAsync(r => r.Statut == "En attente");

        // ── NOTIFICATIONS NON LUES ────────────────────────────────────
        ViewBag.NbNotifsNonLues = await _context.Notifications.CountAsync(n => !n.Lue);

        // ── ABSENCES PAR MATIÈRE (top 5) ──────────────────────────────
        var absByMatiere = await _context.Absences
            .Include(a => a.Matiere)
            .GroupBy(a => a.Matiere!.Nom)
            .Select(g => new { Matiere = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(6)
            .ToListAsync();
        ViewBag.AbsByMatiere = absByMatiere;

        // ── ABSENCES PAR MOIS (12 derniers mois) ─────────────────────
        var now = DateTime.Now;
        var absByMois = new int[12];
        var moisLabels = new string[12];
        string[] moisNoms = { "Jan","Fév","Mar","Avr","Mai","Jun","Jul","Aoû","Sep","Oct","Nov","Déc" };
        for (int i = 11; i >= 0; i--)
        {
            var d = now.AddMonths(-i);
            absByMois[11 - i] = await _context.Absences
                .CountAsync(a => a.DateAbsence.Year == d.Year && a.DateAbsence.Month == d.Month);
            moisLabels[11 - i] = moisNoms[d.Month - 1] + " " + d.Year.ToString()[2..];
        }
        ViewBag.AbsByMois   = absByMois;
        ViewBag.MoisLabels  = moisLabels;

        // ── TOP 5 ÉTUDIANTS LES PLUS ABSENTS ─────────────────────────
        var topAbsents = await _context.Absences
            .Include(a => a.Etudiant)
            .GroupBy(a => new { a.EtudiantId, a.Etudiant!.Nom, a.Etudiant.Prenom })
            .Select(g => new { g.Key.Nom, g.Key.Prenom, Count = g.Count(), NonJust = g.Count(a => !a.Justifiee) })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();
        ViewBag.TopAbsents = topAbsents;

        // ── DERNIÈRES ABSENCES ────────────────────────────────────────
        var dernAbsences = await _context.Absences
            .Include(a => a.Etudiant)
            .Include(a => a.Matiere)
            .Include(a => a.Professeur)
            .OrderByDescending(a => a.DateSaisie)
            .Take(5)
            .ToListAsync();
        ViewBag.DerniersAbsences = dernAbsences;

        ViewBag.Username = User.FindFirst("FullName")?.Value ?? "Admin";
        ViewBag.Role     = role;

        return View();
    }
}