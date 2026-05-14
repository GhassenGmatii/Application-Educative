# Application Éducative - Système de Gestion Universitaire

![Bannière du Projet](https://via.placeholder.com/1200x300.png?text=Application+%C3%89ducative+-+Gestion+et+Suivi)

## 📖 Description du Projet

L'**Application Éducative** est une plateforme moderne de gestion universitaire développée avec **ASP.NET Core MVC**. Elle est conçue pour faciliter la communication et la gestion entre l'administration, les professeurs et les étudiants. 

L'application offre une interface utilisateur élégante basée sur le **Glassmorphism** (effets de transparence et de flou) pour une expérience utilisateur premium. Elle intègre un système robuste pour le suivi des absences, un système de notifications en temps réel (style Facebook), et une gestion complète des réclamations.

## ✨ Fonctionnalités Principales

*   🎓 **Gestion des Étudiants et Professeurs** : Administration complète des profils utilisateurs.
*   📅 **Suivi des Absences** : Enregistrement et consultation des absences par matière, date et heure.
*   🔔 **Système de Notifications Avancé** : Notifications interactives (style Facebook) pour alerter les étudiants lorsqu'ils approchent ou dépassent le seuil d'absence autorisé.
*   ✉️ **Gestion des Réclamations** : Possibilité pour les étudiants de soumettre des réclamations directes aux professeurs ou à l'administration avec suivi du statut.
*   🎨 **Design "Glassmorphism"** : Interface utilisateur moderne, réactive et esthétique.

## 🛠️ Technologies Utilisées

*   **Backend** : C# / ASP.NET Core MVC (.NET 8.0)
*   **Base de données** : Entity Framework Core (SQL Server / InMemory Database pour le développement)
*   **Frontend** : HTML5, CSS3 (Custom Glassmorphism), Razor Views, JavaScript
*   **Architecture** : Modèle-Vue-Contrôleur (MVC)

## 📁 Structure du Projet

*   `Models/` : Contient les entités de l'application (Étudiants, Professeurs, Absences, Réclamations, Notifications).
*   `Views/` : Les interfaces utilisateur en Razor (`.cshtml`), structurées par contrôleur.
*   `Controllers/` : La logique métier reliant les modèles aux vues.
*   `Data/` : Le contexte de la base de données (DbContext) et les configurations Entity Framework.
*   `wwwroot/` : Fichiers statiques (CSS, JS, Images).

## 🚀 Comment Exécuter le Projet

### Prérequis
*   [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)
*   [SDK .NET 8.0](https://dotnet.microsoft.com/download) (ou version ultérieure)
*   SQL Server (optionnel, selon la configuration de la chaîne de connexion)

### Étapes de lancement

1.  **Cloner ou télécharger** le projet sur votre machine locale.
2.  **Ouvrir le projet** : Double-cliquez sur le fichier `Application Educative.sln` pour l'ouvrir dans Visual Studio.
3.  **Restaurer les packages NuGet** : Visual Studio le fera automatiquement à l'ouverture, ou vous pouvez exécuter la commande `dotnet restore` dans le terminal.
4.  **Mettre à jour la base de données** :
    *   Ouvrez la *Console du Gestionnaire de package* (Tools > NuGet Package Manager > Package Manager Console).
    *   Exécutez la commande : `Update-Database`
5.  **Lancer l'application** :
    *   Appuyez sur `F5` ou cliquez sur le bouton "Exécuter" (IIS Express ou Profil du projet).
    *   L'application s'ouvrira dans votre navigateur par défaut (généralement sur `http://localhost:xxxx`).

## 🤝 Contribution

Si vous souhaitez contribuer à ce projet, n'hésitez pas à créer une branche et à soumettre une Pull Request.

---
*Développé avec passion pour l'éducation moderne.*
