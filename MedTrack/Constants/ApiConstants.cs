using System;
using System.Collections.Generic;
using System.Text;

namespace MedTrack.Constants
{
    internal class ApiConstants
    {
        // URL de base de votre API
        public const string BaseUrl = "https://med-track-back.vercel.app/api/";

        // Endpoints d'authentification
        public const string SignupEndpoint = "auth/signup";
        public const string LoginEndpoint = "auth/login";
        public const string RefreshTokenEndpoint = "auth/refresh";

        // Endpoints de profil
        public const string ProfilEndpoint = "profil";

        // Endpoints de documents
        public const string DocumentsEndpoint = "documents";

        // Endpoints de médicaments
        public const string MedicamentsEndpoint = "medicaments";

        // Endpoints de rendez-vous
        public const string RendezVousEndpoint = "rendezvous";

        // Clés de stockage sécurisé
        public const string AccessTokenKey = "auth_token";
        public const string RefreshTokenKey = "refresh_token";

        // Timeout des requêtes HTTP (en secondes)
        public const int HttpTimeout = 30;
    }
}
