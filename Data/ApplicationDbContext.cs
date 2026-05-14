using Application_Educative.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Educative.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Matiere> Matieres { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Absence> Absences { get; set; }
        public DbSet<Professeur> Professeurs { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ProfesseurSection> ProfesseurSections { get; set; }
        public DbSet<Reclamation> Reclamations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Absence relations
            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Etudiant)
                .WithMany(e => e.Absences)
                .HasForeignKey(a => a.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Professeur)
                .WithMany()
                .HasForeignKey(a => a.ProfesseurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Matiere)
                .WithMany(m => m.Absences)
                .HasForeignKey(a => a.MatiereId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Absence>()
                .HasOne(a => a.Section)
                .WithMany()
                .HasForeignKey(a => a.SectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}