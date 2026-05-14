using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Matiere
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = "";

        // ✅ Description supprimé pour éviter le conflit

        public ICollection<Absence> Absences { get; set; } = new List<Absence>();
    }
}