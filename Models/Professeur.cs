using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Professeur
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; } = "";

        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        // ✅ Mot de passe pour connexion
        public string? Password { get; set; }

        public string? Telephone { get; set; }

        [Display(Name = "Spécialité")]
        public string? Specialite { get; set; }

        public DateTime DateAjout { get; set; } = DateTime.Now;
        public DateTime? DateModification { get; set; }

        public ICollection<ProfesseurSection> ProfesseurSections { get; set; } = new List<ProfesseurSection>();
    }
}