using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit avoir entre 3 et 100 caractères")]
        [Display(Name = "Nom d'utilisateur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit avoir au moins 6 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        // ✅ Optionnel (corrigé)
        [Display(Name = "Prénom")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        // ✅ Optionnel (corrigé)
        [Display(Name = "Nom")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Display(Name = "Actif")]
        public bool EstActif { get; set; } = true;

        [Display(Name = "Date d'ajout")]
        public DateTime DateAjout { get; set; } = DateTime.Now;
        [Display(Name = "Date de modification")]
        public DateTime? DateModification { get; set; }
    }
}