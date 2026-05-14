namespace Application_Educative.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";       // ex: "Section A"
        public string? Niveau { get; set; }          // ex: "2ème année Info"

        public ICollection<ProfesseurSection> ProfesseurSections { get; set; } = new List<ProfesseurSection>();
    }
}