namespace Application_Educative.Models
{
    public class ProfesseurSection
    {
        public int Id { get; set; }

        public int ProfesseurId { get; set; }
        public Professeur? Professeur { get; set; }

        public int SectionId { get; set; }
        public Section? Section { get; set; }

        public int NombreHeures { get; set; }
        public string? Jour { get; set; }          
        public TimeSpan? HeureDebut { get; set; }
        public TimeSpan? HeureFin { get; set; }
        public DateTime? DateDebut { get; set; }
    }
}