using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application_Educative.Models
{
    public class Etudiant
    {
        [Key]
        public int EtudiantId { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirmation requise")]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "L'état est requis")]
        public string Etat { get; set; }

        [Required(ErrorMessage = "La section est requise")]
        public string Section { get; set; }

        [Required(ErrorMessage = "La spécialité est requise")]
        public string Specialite { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public DateTime? DateModification { get; set; }

        public ICollection<Absence> Absences { get; set; } = new List<Absence>();
    }
}