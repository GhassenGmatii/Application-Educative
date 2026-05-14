using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Absence
    {
        public int Id { get; set; }

        [Required]
        public int EtudiantId { get; set; }
        public Etudiant? Etudiant { get; set; }

        [Required]
        public int ProfesseurId { get; set; }
        public Professeur? Professeur { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        [Required]
        public int MatiereId { get; set; }
        public Matiere? Matiere { get; set; }

        [Required]
        public DateTime DateAbsence { get; set; }

        public bool Justifiee { get; set; } = false;

        [StringLength(500)]
        public string? Commentaire { get; set; }

        public DateTime DateSaisie { get; set; } = DateTime.Now;
    }
}