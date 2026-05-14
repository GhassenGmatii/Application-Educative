using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Application_Educative.Controllers
{
    public class GestionAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GestionAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /GestionAdmin/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var admins = await _context.Admins.ToListAsync();
            ViewBag.CurrentUserEmail = "admin@gmail.com";
            return View(admins);
        }

        // GET: /GestionAdmin/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Admin());
        }

        // POST: /GestionAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admin admin)
        {
            var existingAdmin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == admin.Username);

            if (existingAdmin != null)
            {
                ModelState.AddModelError("Username", "❌ Cet email est déjà utilisé !");
            }

            if (ModelState.IsValid)
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
                admin.DateAjout = DateTime.Now;
                _context.Add(admin);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Admin ajouté avec succès !";
                return RedirectToAction(nameof(Index));
            }

            return View(new Admin());
        }

        // GET: /GestionAdmin/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        // POST: /GestionAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Admin admin)
        {
            if (id != admin.Id) return NotFound();

            try
            {
                var adminToUpdate = await _context.Admins.FindAsync(id);
                if (adminToUpdate == null) return NotFound();

                // Vérifier si le nouvel email est déjà utilisé par un autre admin
                var emailExiste = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == admin.Username && a.Id != id);
                if (emailExiste != null)
                {
                    TempData["ErrorMessage"] = "❌ Cet email est déjà utilisé par un autre admin !";
                    return View(adminToUpdate);
                }

                // Mettre à jour tous les champs modifiables
                adminToUpdate.Username = admin.Username;
                adminToUpdate.FirstName = admin.FirstName;
                adminToUpdate.LastName = admin.LastName;
                adminToUpdate.DateModification = DateTime.Now;

                // Mot de passe seulement si fourni
                if (!string.IsNullOrEmpty(admin.Password))
                {
                    adminToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Admin modifié avec succès !";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur : {ex.Message}");
                return View(admin);
            }
        }

        // GET: /GestionAdmin/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            return View(admin);
        }

        // POST: /GestionAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "✅ Admin supprimé avec succès !";
            return RedirectToAction(nameof(Index));
        }

        // GET: /GestionAdmin/Search
        [HttpGet]
        public async Task<IActionResult> Search(string email)
        {
            var admins = await _context.Admins.ToListAsync();
            ViewBag.CurrentUserEmail = "admin@gmail.com";

            if (!string.IsNullOrEmpty(email))
            {
                admins = admins
                    .Where(a => a.Username.Contains(email, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ViewBag.SearchTerm = email;
                ViewBag.SearchPerformed = true;
            }

            return View("Index", admins);
        }

        // GET: /GestionAdmin/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();
            return View(admin);
        }
    }
}