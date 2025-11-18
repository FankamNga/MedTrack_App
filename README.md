MedTrack — Frontend (.NET MAUI)

Statut : Prototype fonctionnel (frontend .NET MAUI)
Nom de l’application : MedTrack
But : Application mobile pour centraliser le suivi médical (médicaments, rendez-vous, documents, profil santé).

Description courte

MedTrack est le frontend mobile développé en .NET MAUI (architecture MVVM) qui consomme une API REST (backend Node.js + Express / Postgres). L’application fournit les pages d’authentification, gestion des médicaments, rendez-vous, documents et un profil santé. Conçue pour être simple, accessible et légère.

Table des matières

Installation (pré-requis & exécution)

Fonctionnalités

Architecture & structure du projet

API attendue (endpoints)

Configuration (variables)

Tests & validation

UI / Accessibilité / Écoconception

Déploiement / Packaging

Contribution

Licence


1. Installation
Prérequis

.NET SDK (8.0+ recommandé) installé.

Workloads MAUI installés : dotnet workload install maui

(Pour développement Android) Android SDK + émulateur / téléphone USB.

(Pour iOS) macOS + Xcode (si nécessaire).

Node.js / backend accessible (ou exécuter localement le backend fourni séparément).

Cloner le dépôt
git clone https://github.com/FankamNga/MedTrack_App
cd medtrack-frontend-maui

Configuration (locale)

Copier le fichier de configuration modèle :

cp appsettings.example.json appsettings.local.json


Modifier appsettings.local.json pour y mettre l’URL de l’API :

{
  "ApiBaseUrl": "https://localhost:3000/api"
}


Remplacez par l’URL de votre backend (https si possible).

Restaurer et lancer
dotnet restore
dotnet build

Lancer sur Android (émulateur / appareil)
dotnet build -f net8.0-android
dotnet run -f net8.0-android

Lancer sur Windows (si ciblé)
dotnet build -f net8.0-windows10.0.19041.0
dotnet run -f net8.0-windows10.0.19041.0

2. Fonctionnalités (v1 - version de base)

Authentification : inscription & connexion (JWT).

Médicaments : CRUD (nom, dosage, fréquence).

Rendez-vous : CRUD (date, heure, lieu, type).

Documents médicaux : upload (image/pdf), liste, visualisation plein écran.

Profil santé : nom, âge, sexe, groupe sanguin, allergies/maladies.

Accueil (Dashboard) : vue synthétique + gros boutons d’accès.

Navigation simple : Vue Liste → Ajouter / Modifier → Retour.

3. Architecture & structure du projet
Pattern

MVVM : Models, ViewModels, Views.

Service layer pour appels API et stockage local (preferences / secure storage).

Utilisation de CommunityToolkit.Mvvm recommandée.

Arborescence (exemple)
/src
  /MedTrack.App
    /Views
      LoginPage.xaml
      RegisterPage.xaml
      DashboardPage.xaml
      MedicinesListPage.xaml
      MedicineEditPage.xaml
      AppointmentsListPage.xaml
      AppointmentEditPage.xaml
      DocumentsPage.xaml
      ProfilePage.xaml
    /ViewModels
      LoginViewModel.cs
      RegisterViewModel.cs
      DashboardViewModel.cs
      MedicinesViewModel.cs
      MedicineEditViewModel.cs
      AppointmentsViewModel.cs
      DocumentsViewModel.cs
      ProfileViewModel.cs
    /Models
      User.cs
      Medicine.cs
      Appointment.cs
      Document.cs
      HealthProfile.cs
    /Services
      IApiService.cs
      ApiService.cs
      IStorageService.cs
      SecureStorageService.cs
    /Resources
      Styles.xaml
      Strings.resx
    App.xaml
    MainPage.xaml

Principes clés

ViewModels contiennent la logique, utilisent INotifyPropertyChanged.

Services gèrent la communication réseau et la persistance locale légère.

Toutes les requêtes réseau passent par ApiService qui gère l’injection du token JWT.

4. API attendue (exemples d’endpoints)

Le backend doit exposer une API REST JSON. Voici les endpoints attendus par le frontend :

Authentification

POST /api/auth/register — { email, password, name } => 201 + { token, user }

POST /api/auth/login — { email, password } => { token, user }

Médicaments

GET /api/medicines — liste

GET /api/medicines/{id} — détails

POST /api/medicines — créer { name, dosage, frequency }

PUT /api/medicines/{id} — modifier

DELETE /api/medicines/{id} — supprimer

Rendez-vous

GET /api/appointments

POST /api/appointments — { datetime, location, type, note }

PUT /api/appointments/{id}

DELETE /api/appointments/{id}

Documents

GET /api/documents

POST /api/documents — multipart/form-data (file + metadata type)

GET /api/documents/{id} — retourne URL ou contenu

Profil

GET /api/profile

PUT /api/profile — modifier informations santé

Sécurité

Les endpoints protégés exigent header Authorization: Bearer <token>.

5. Configuration & stockage
Fichier de config (exemple)

appsettings.example.json

{
  "ApiBaseUrl": "https://localhost:3000/api",
  "TimeoutSeconds": 30
}

Stockage côté client

Token JWT : stocker dans SecureStorage (MAUI Essentials / Microsoft.Maui.Storage).

Données non sensibles (préférences) : Preferences.

Sécurité

Utiliser HTTPS pour toutes les requêtes en production.

Ne pas stocker de mots de passe en clair.

Protéger l’upload/download des documents (vérifier types et tailles côté serveur).

6. Tests & validation
Tests unitaires

Créer un projet de test MedTrack.Tests pour ViewModels.

Tester la logique d’ajout/modification/suppression dans ViewModels via moq des IApiService.

Tests fonctionnels manuels

Vérifier CRUD pour chaque ressource.

Tester authentification / accès restreint.

Vérifier chargement et affichage des documents.

Critères d’acceptation (extraits)

Temps de chargement d’une page < 2s (dépend du backend).

Navigation fluide, boutons accessibles.

Mot de passe chiffré et accès uniquement après login.

7. UI / Accessibilité / Écoconception

UX minimaliste : gros boutons, textes lisibles, contraste suffisant.

Éviter animations lourdes ; images optimisées.

Thème coloré sobre (bleu/gris) pour usage médical.

Polices accessibles; tailles par défaut 16+ pour textes principaux.

Support des tailles d’écran et orientation.

8. Déploiement / Packaging

Générer APK/AAB pour Android via les outils MAUI / dotnet.

Pour iOS, suivre le processus de build macOS/Xcode + provisioning.

Mettre la configuration backend en variables d’environnement ou dans appsettings pour différencier dev/prod.

9. Contribution

Fork → branch feature/your-feature → PR.

Respecter le pattern MVVM.

Ajouter tests unitaires pour toute nouvelle ViewModel.

Documenter chaque nouveau service / endpoint utilisé.

10. Licence

Précisez la licence du projet (ex: MIT). Exemple :

MIT License
