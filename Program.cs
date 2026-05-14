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
            new Section { Nom = "Section A", Niveau = "1ère année" },
            new Section { Nom = "Section B", Niveau = "1ère année" },
            new Section { Nom = "Section A", Niveau = "2ème année" },
            new Section { Nom = "Section B", Niveau = "2ème année" },
            new Section { Nom = "Section A", Niveau = "3ème année" },
            new Section { Nom = "Section B", Niveau = "3ème année" }
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
}

app.Run();