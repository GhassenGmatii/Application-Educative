using Application_Educative.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Educative.Data
{
    public static class TestDataSeeder
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Générer des données riches si pas assez d'étudiants (pour les tests)
            if (context.Etudiants.Count() < 20)
            {
                SeedRichData(context);
            }

            if (context.Absences.Any())
            {
                return;   // La BD a déjà été initialisée pour les absences
            }

            // Récupérer les données existantes
            var etudiants = context.Etudiants.ToList();
            var professeurs = context.Professeurs.ToList();
            var matieres = context.Matieres.ToList();
            var sections = context.Sections.ToList();
            var admin = context.Admins.FirstOrDefault();

            if (!etudiants.Any() || !professeurs.Any() || !matieres.Any() || !sections.Any() || admin == null)
            {
                return; // Les données de base ne sont pas encore là (gérées dans Program.cs)
            }

            var random = new Random();

            // ============================================
            // 1. Génération de 30 Absences fictives
            // ============================================
            var absences = new List<Absence>();
            for (int i = 1; i <= 30; i++)
            {
                var etudiant = etudiants[random.Next(etudiants.Count)];
                var professeur = professeurs[random.Next(professeurs.Count)];
                var matiere = matieres[random.Next(matieres.Count)];
                
                // Trouver la bonne section pour l'étudiant
                string expectedNiveau = "";
                string expectedNom = "";
                if (etudiant.Section != null) {
                    if (etudiant.Section.Contains("1ère Année")) expectedNiveau = "1ère année";
                    else if (etudiant.Section.Contains("2ème Année")) expectedNiveau = "2ème année";
                    else if (etudiant.Section.Contains("3ème Année")) expectedNiveau = "3ème année";
                    
                    if (etudiant.Section.Contains("Licence")) expectedNom = "Cours du Jour";
                    else if (etudiant.Section.Contains("Master")) expectedNom = "Master";
                    else if (etudiant.Section.Contains("Cycle Ing")) expectedNom = "Cycle Ingénieur Alternant";
                }
                
                var section = sections.FirstOrDefault(s => s.Niveau == expectedNiveau && s.Nom == expectedNom) ?? sections[random.Next(sections.Count)];

                var estJustifiee = random.Next(10) > 6; // 30% de chances d'être justifiée

                absences.Add(new Absence
                {
                    EtudiantId = etudiant.EtudiantId,
                    ProfesseurId = professeur.Id,
                    MatiereId = matiere.Id,
                    SectionId = section.Id,
                    DateAbsence = DateTime.Now.AddDays(-random.Next(1, 60)), // Entre hier et il y a 60 jours
                    Justifiee = estJustifiee,
                    Commentaire = estJustifiee ? "Certificat médical fourni" : "Aucun justificatif",
                    DateSaisie = DateTime.Now.AddDays(-random.Next(1, 30))
                });
            }
            context.Absences.AddRange(absences);
            context.SaveChanges(); // On sauvegarde pour obtenir les Ids

            // ============================================
            // 2. Génération de Reclamations
            // ============================================
            var reclamations = new List<Reclamation>();
            for (int i = 0; i < 5; i++)
            {
                var etudiant = etudiants[random.Next(etudiants.Count)];
                var absence = absences[random.Next(absences.Count)];
                var statut = i % 2 == 0 ? "En attente" : "Traitée";

                reclamations.Add(new Reclamation
                {
                    EtudiantId = etudiant.EtudiantId,
                    AbsenceId = absence.Id,
                    DestinataireRole = "Admin",
                    DestinataireId = admin.Id,
                    Message = $"Bonjour, je conteste l'absence du {absence.DateAbsence:dd/MM/yyyy} car j'étais présent au cours.",
                    Statut = statut,
                    DateEnvoi = DateTime.Now.AddDays(-random.Next(1, 10)),
                    ReponseAdmin = statut == "Traitée" ? "Après vérification, votre présence a été confirmée." : null,
                    DateReponse = statut == "Traitée" ? DateTime.Now.AddDays(-1) : null
                });
            }
            context.Reclamations.AddRange(reclamations);
            context.SaveChanges();

            // ============================================
            // 3. Génération de Notifications
            // ============================================
            var notifications = new List<Notification>();
            foreach (var etudiant in etudiants)
            {
                // Notifier les étudiants de leurs absences non justifiées
                var etudiantAbsences = absences.Where(a => a.EtudiantId == etudiant.EtudiantId && !a.Justifiee).ToList();
                foreach (var abs in etudiantAbsences.Take(2)) // 2 notifications max par étudiant
                {
                    notifications.Add(new Notification
                    {
                        Message = $"Une nouvelle absence a été enregistrée en {abs.Matiere?.Nom ?? "Matière"} le {abs.DateAbsence:dd/MM/yyyy}.",
                        DateCreation = DateTime.Now.AddDays(-random.Next(1, 5)),
                        Lue = random.Next(2) == 0,
                        Lien = "/Etudiant/Absences",
                        DestinataireRole = "Etudiant",
                        DestinataireId = etudiant.EtudiantId,
                        Type = "absence"
                    });
                }
            }
            context.Notifications.AddRange(notifications);
            context.SaveChanges();
        }

        private static void SeedRichData(ApplicationDbContext context)
        {
            var random = new Random();
            
            // 1. Génération de Professeurs Riches
            var prenomsProf = new[] { "Nizar", "Karim", "Wafa", "Hajer", "Zied", "Mourad", "Salma", "Olfa", "Mehdi", "Ines" };
            var nomsProf = new[] { "Gharbi", "Trabelsi", "Ben Youssef", "Kallel", "Jaziri", "Bouazizi", "Cherif", "Ammar", "Triki", "Zouari" };
            var specialites = new[] { "Informatique", "Mathématiques", "Physique", "Gestion", "Intelligence Artificielle" };

            var nouveauxProfs = new List<Professeur>();
            for (int i = 0; i < 10; i++)
            {
                nouveauxProfs.Add(new Professeur
                {
                    FirstName = prenomsProf[i % prenomsProf.Length],
                    LastName = nomsProf[i % nomsProf.Length],
                    Email = $"{prenomsProf[i].ToLower()}.{nomsProf[i].ToLower()}@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Specialite = specialites[i % specialites.Length],
                    Telephone = $"9{random.Next(1000000, 9999999)}"
                });
            }
            context.Professeurs.AddRange(nouveauxProfs);
            context.SaveChanges();

            var tousProfs = context.Professeurs.ToList();
            var toutesSections = context.Sections.ToList();
            var toutesMatieres = context.Matieres.ToList();

            // Affectation des professeurs à plusieurs sections
            var profSections = new List<ProfesseurSection>();
            foreach (var prof in tousProfs)
            {
                // Chaque prof enseigne dans 2 à 4 classes
                int nbClasses = random.Next(2, 5);
                for (int c = 0; c < nbClasses; c++)
                {
                    var section = toutesSections[random.Next(toutesSections.Count)];
                    var matiere = toutesMatieres[random.Next(toutesMatieres.Count)];
                    
                    // Vérifier que la combinaison n'existe pas déjà pour ce prof
                    if (!profSections.Any(ps => ps.ProfesseurId == prof.Id && ps.SectionId == section.Id && ps.MatiereId == matiere.Id))
                    {
                        profSections.Add(new ProfesseurSection
                        {
                            ProfesseurId = prof.Id,
                            SectionId = section.Id,
                            MatiereId = matiere.Id,
                            Specialite = prof.Specialite,
                            NombreHeures = random.Next(2, 6),
                            Jour = new[] { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi" }[random.Next(5)]
                        });
                    }
                }
            }
            context.ProfesseurSections.AddRange(profSections);
            context.SaveChanges();

            // 2. Génération de 50 Étudiants
            var prenomsEtu = new[] { "Youssef", "Omar", "Ayoub", "Rayen", "Aziz", "Malek", "Nour", "Mariem", "Yassmine", "Chaima", "Farah", "Sirine", "Wassim", "Iheb", "Mahdi" };
            var nomsEtu = new[] { "Ben Ali", "Trabelsi", "Gharbi", "Driss", "Jlassi", "Ben Salem", "Boussetta", "Khemiri", "Ayari", "Riahi", "Oueslati", "Mansour" };
            
            var sectionsEtu = new[] { "1ère Année Licence", "2ème Année Licence", "3ème Année Licence", "1ère Année Master", "2ème Année Master", "1ère Année Cycle Ing", "2ème Année Cycle Ing" };
            var specsEtu = new[] { "Informatique", "Mathématiques", "Physique", "Gestion", "Intelligence Artificielle", "Cybersécurité" };

            var nouveauxEtudiants = new List<Etudiant>();
            for (int i = 0; i < 50; i++)
            {
                var prenom = prenomsEtu[random.Next(prenomsEtu.Length)];
                var nom = nomsEtu[random.Next(nomsEtu.Length)];
                var sectionAssigned = sectionsEtu[random.Next(sectionsEtu.Length)];
                var specAssigned = specsEtu[random.Next(specsEtu.Length)];

                nouveauxEtudiants.Add(new Etudiant
                {
                    Nom = nom,
                    Prenom = prenom,
                    Email = $"etu{i}_{prenom.ToLower()}@gmail.com",
                    MotDePasse = BCrypt.Net.BCrypt.HashPassword("1234"),
                    ConfirmPassword = "1234",
                    Etat = "Actif",
                    Section = sectionAssigned,
                    Specialite = specAssigned
                });
            }
            context.Etudiants.AddRange(nouveauxEtudiants);
            context.SaveChanges();
        }
    }
}
