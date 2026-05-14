// ════════════════════════════════════════════════════════════════
//  FICHIER 1 : Models/Notification.cs
//  Remplace ou complète votre modèle existant
// ════════════════════════════════════════════════════════════════
using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class Notification
    {
        public int Id { get; set; }

        /// <summary>Le texte affiché à l'utilisateur.</summary>
        [Required]
        [StringLength(500)]
        public string Message { get; set; } = "";

        /// <summary>"Etudiant", "Professeur", "Admin"</summary>
        [Required]
        public string DestinataireRole { get; set; } = "";

        /// <summary>Id de l'étudiant / professeur / admin concerné.</summary>
        public int DestinataireId { get; set; }

        /// <summary>"absence", "justification", "info"</summary>
        public string Type { get; set; } = "info";

        /// <summary>Lien cliquable optionnel (ex: "/Absence/Index")</summary>
        public string? Lien { get; set; }

        public bool Lue { get; set; } = false;

        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}


// ════════════════════════════════════════════════════════════════
//  FICHIER 2 : Snippet _Layout.cshtml  — Badge cloche dans la navbar
//  Collez ce bloc dans votre <nav> là où vous voulez la cloche
// ════════════════════════════════════════════════════════════════

/*
Ajoutez dans votre _Layout.cshtml (dans la balise <nav>) :

<a href="/Notification/Index" class="notif-bell" id="notifBell" title="Notifications">
    <i class="fas fa-bell"></i>
    <span class="notif-bubble" id="notifBubble" style="display:none">0</span>
</a>

<style>
.notif-bell {
    position: relative;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 38px; height: 38px;
    border-radius: 10px;
    background: rgba(249,115,22,.12);
    color: #F97316;
    font-size: 1rem;
    text-decoration: none;
    transition: background .2s;
}
.notif-bell:hover { background: rgba(249,115,22,.22); }
.notif-bubble {
    position: absolute;
    top: -4px; right: -4px;
    min-width: 18px; height: 18px;
    border-radius: 50px;
    background: #EF4444;
    color: white;
    font-size: .62rem;
    font-weight: 900;
    display: flex !important;
    align-items: center;
    justify-content: center;
    padding: 0 4px;
    border: 2px solid white;
    font-family: 'Nunito', sans-serif;
}
</style>

<script>
// Polling toutes les 30 secondes pour mettre à jour le badge
(function pollNotifs() {
    fetch('/Notification/NombreNonLues')
        .then(r => r.json())
        .then(data => {
            const bubble = document.getElementById('notifBubble');
            if (!bubble) return;
            if (data.count > 0) {
                bubble.textContent = data.count > 9 ? '9+' : data.count;
                bubble.style.display = 'flex';
            } else {
                bubble.style.display = 'none';
            }
        })
        .catch(() => {});
    setTimeout(pollNotifs, 30000);
})();
</script>
*/


// ════════════════════════════════════════════════════════════════
//  FICHIER 3 : Résumé des routes à ajouter / vérifier dans Program.cs
// ════════════════════════════════════════════════════════════════

/*
Dans Program.cs, assurez-vous que le routing MVC par défaut couvre bien :

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Les routes utilisées par ce système :

GET  /Notification/Index            → page notifications + absences étudiant
POST /Notification/MarquerLue/{id}  → marquer 1 notif lue (AJAX)
POST /Notification/MarquerToutLu    → marquer toutes lues
GET  /Notification/NombreNonLues    → badge navbar (AJAX polling)
*/