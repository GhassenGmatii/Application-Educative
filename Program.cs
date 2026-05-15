using Application_Educative.Data;
using Application_Educative.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;

var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
var contentRoot = Path.GetDirectoryName(exePath) ?? Directory.GetCurrentDirectory();

// Remonter dans l'arborescence jusqu'à trouver le dossier wwwroot du projet
var dir = new DirectoryInfo(contentRoot);
while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "wwwroot")))
{
    dir = dir.Parent;
}

if (dir != null)
{
    contentRoot = dir.FullName;
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = Path.Combine(contentRoot, "wwwroot")
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    // Fix existing students with dummy sections
    var etudiantsToFix = context.Etudiants.Where(e => e.Section == "Section A" || e.Section == "Section B").ToList();
    if (etudiantsToFix.Any())
    {
        foreach (var e in etudiantsToFix)
        {
            if (e.Section == "Section A") e.Section = "1ère Année Licence";
            if (e.Section == "Section B") e.Section = "2ème Année Licence";
        }
        context.SaveChanges();
    }

    if (!context.Admins.Any())
    {
        context.Admins.Add(new Admin
        {
            Username = "admin@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("1234"),
            FirstName = "Admin",
            LastName = "Principal",
            DateAjout = DateTime.Now,
            EstActif = true
        });
        context.SaveChanges();
    }

    if (!context.Sections.Any())
    {
        context.Sections.AddRange(
            new Section { Nom = "Cycle Ingénieur Alternant", Niveau = "1ère année" },
            new Section { Nom = "Cycle Ingénieur Alternant", Niveau = "2ème année" },
            new Section { Nom = "Cycle Ingénieur Alternant", Niveau = "3ème année" },
            new Section { Nom = "Cours du Jour", Niveau = "1ère année" },
            new Section { Nom = "Cours du Jour", Niveau = "2ème année" },
            new Section { Nom = "Cours du Jour", Niveau = "3ème année" },
            new Section { Nom = "Master", Niveau = "1ère année" },
            new Section { Nom = "Master", Niveau = "2ème année" }
        );
        context.SaveChanges();
    }

    // ✅ NOUVEAU — Matières par défaut
    if (!context.Matieres.Any())
    {
        context.Matieres.AddRange(
            new Matiere { Nom = "Mathématiques" },
            new Matiere { Nom = "Informatique" },
            new Matiere { Nom = "Physique" },
            new Matiere { Nom = "Anglais" },
            new Matiere { Nom = "Français" },
            new Matiere { Nom = "Algorithmique" },
            new Matiere { Nom = "Base de données" },
            new Matiere { Nom = "Réseaux" }
        );
        context.SaveChanges();
    }

    if (!context.Professeurs.Any())
    {
        context.Professeurs.AddRange(
            new Professeur { FirstName = "Ahmed", LastName = "Ben Ali", Email = "ahmed.benali@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("1234"), Specialite = "Mathématiques", Telephone = "22334455" },
            new Professeur { FirstName = "Sami", LastName = "Trabelsi", Email = "sami.trabelsi@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("1234"), Specialite = "Informatique", Telephone = "99887766" },
            new Professeur { FirstName = "Nadia", LastName = "Mansour", Email = "nadia.mansour@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("1234"), Specialite = "Physique", Telephone = "55443322" }
        );
        context.SaveChanges();
    }

    if (!context.Etudiants.Any())
    {
        context.Etudiants.AddRange(
            new Etudiant { Nom = "Gmati", Prenom = "Ghassen", Email = "ghassen@gmail.com", MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"), ConfirmPassword = "1234", Etat = "Actif", Section = "1ère Année Licence", Specialite = "Informatique" },
            new Etudiant { Nom = "Jlassi", Prenom = "Mohamed", Email = "mohamed@gmail.com", MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"), ConfirmPassword = "1234", Etat = "Actif", Section = "1ère Année Licence", Specialite = "Informatique" },
            new Etudiant { Nom = "Ayari", Prenom = "Amina", Email = "amina@gmail.com", MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"), ConfirmPassword = "1234", Etat = "Actif", Section = "1ère Année Licence", Specialite = "Mathématiques" },
            new Etudiant { Nom = "Boussetta", Prenom = "Sarra", Email = "sarra@gmail.com", MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"), ConfirmPassword = "1234", Etat = "Actif", Section = "2ème Année Licence", Specialite = "Physique" },
            new Etudiant { Nom = "Khemiri", Prenom = "Yassine", Email = "yassine@gmail.com", MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"), ConfirmPassword = "1234", Etat = "Actif", Section = "1ère Année Licence", Specialite = "Informatique" }
        );
        context.SaveChanges();
    }

    // Générer les données de test supplémentaires (Absences, Réclamations, Notifications)
    TestDataSeeder.Initialize(context);
}

app.Run();