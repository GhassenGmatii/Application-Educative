using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;

namespace Application_Educative.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email et mot de passe sont obligatoires.");
                return View("~/Views/Account/Login.cshtml");
            }

            try
            {
                string role = "";
                string fullName = "";
                string userId = "";

                // 1. Vérifier Admin
                var admin = _context.Admins.FirstOrDefault(a => a.Username == username);
                if (admin != null && BCrypt.Net.BCrypt.Verify(password, admin.Password))
                {
                    role = "Admin";
                    fullName = $"{admin.FirstName} {admin.LastName}";
                    userId = admin.Id.ToString();
                }

                // 2. Vérifier Professeur
                if (string.IsNullOrEmpty(role))
                {
                    var prof = _context.Professeurs.FirstOrDefault(p => p.Email == username);
                    if (prof != null && !string.IsNullOrEmpty(prof.Password) &&
                        BCrypt.Net.BCrypt.Verify(password, prof.Password))
                    {
                        role = "Professeur";
                        fullName = $"{prof.FirstName} {prof.LastName}";
                        userId = prof.Id.ToString();
                    }
                }

                // 3. Vérifier Étudiant
                if (string.IsNullOrEmpty(role))
                {
                    var etudiant = _context.Etudiants.FirstOrDefault(e => e.Email == username);
                    if (etudiant != null && BCrypt.Net.BCrypt.Verify(password, etudiant.MotDePasse))
                    {
                        role = "Etudiant";
                        fullName = $"{etudiant.Prenom} {etudiant.Nom}";
                        userId = etudiant.EtudiantId.ToString();
                    }
                }

                if (string.IsNullOrEmpty(role))
                {
                    ModelState.AddModelError("", "Email ou mot de passe incorrect.");
                    return View("~/Views/Account/Login.cshtml");
                }

                // Créer les claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("FullName", fullName),
                    new Claim("UserId", userId)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirection selon rôle
                return role switch
                {
                    "Admin" => RedirectToAction("Index", "Home"),
                    "Professeur" => RedirectToAction("Dashboard", "Professeur"),
                    "Etudiant" => RedirectToAction("Dashboard", "Etudiant"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur : {ex.Message}");
                return View("~/Views/Account/Login.cshtml");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}