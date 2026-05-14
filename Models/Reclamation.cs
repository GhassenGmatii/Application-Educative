using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Reclamation
    {
        public int Id { get; set; }

        [Required]
        public int EtudiantId { get; set; }
        public Etudiant? Etudiant { get; set; }

        /// <summary>Identifiant de l'absence concernée (optionnel)</summary>
        public int? AbsenceId { get; set; }
        public Absence? Absence { get; set; }

        /// <summary>"Admin" ou "Professeur"</summary>
        [Required]
        public string DestinataireRole { get; set; } = "Admin";

        /// <summary>Id du destinataire (admin ou professeur)</summary>
        public int DestinataireId { get; set; }

        [Required(ErrorMessage = "Le message est obligatoire")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Le message doit contenir entre 10 et 1000 caractères")]
        public string Message { get; set; } = "";

        /// <summary>Statut : "En attente", "Traitée", "Rejetée"</summary>
        public string Statut { get; set; } = "En attente";

        public DateTime DateEnvoi { get; set; } = DateTime.Now;

        public string? ReponseAdmin { get; set; }
        public DateTime? DateReponse { get; set; }
    }
}
