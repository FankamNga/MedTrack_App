using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MedTrack.Models
{
    public class ApiModels
    {
        public class User
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("aud")]
            public string Aud { get; set; }

            [JsonPropertyName("role")]
            public string Role { get; set; }

            [JsonPropertyName("email_confirmed_at")]
            public string EmailConfirmedAt { get; set; }

            [JsonPropertyName("phone")]
            public string Phone { get; set; }

            [JsonPropertyName("confirmed_at")]
            public string ConfirmedAt { get; set; }

            [JsonPropertyName("last_sign_in_at")]
            public string LastSignInAt { get; set; }

            [JsonPropertyName("created_at")]
            public string CreatedAt { get; set; }

            [JsonPropertyName("updated_at")]
            public string UpdatedAt { get; set; }

            [JsonPropertyName("is_anonymous")]
            public bool IsAnonymous { get; set; }
        }

        public class Session
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("expires_at")]
            public long ExpiresAt { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("user")]
            public User User { get; set; }

            [JsonPropertyName("weak_password")]
            public object WeakPassword { get; set; }
        }

        public class AuthResponse
        {
            [JsonPropertyName("user")]
            public User User { get; set; }

            [JsonPropertyName("session")]
            public Session Session { get; set; }
        }

        public class LoginRequest
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        public class SignupRequest
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }
            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        // ==================== PROFIL ====================

        public class Profil
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("age")]
            public int? Age { get; set; }

            [JsonPropertyName("sexe")]
            public string Sexe { get; set; }

            [JsonPropertyName("groupeSanguin")]
            public string GroupeSanguin { get; set; }

            [JsonPropertyName("allergies")]
            public string Allergies { get; set; }

            [JsonPropertyName("maladies")]
            public string Maladies { get; set; }

            [JsonPropertyName("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("updatedAt")]
            public DateTime UpdatedAt { get; set; }
        }

        public class UpdateProfilRequest
        {
            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("age")]
            public int? Age { get; set; }

            [JsonPropertyName("sexe")]
            public string Sexe { get; set; }

            [JsonPropertyName("groupeSanguin")]
            public string GroupeSanguin { get; set; }

            [JsonPropertyName("allergies")]
            public string Allergies { get; set; }

            [JsonPropertyName("maladies")]
            public string Maladies { get; set; }
        }

        // ==================== DOCUMENTS ====================

        public class Document
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("typeDocument")]
            public string TypeDocument { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("createdAt")]
            public DateTime CreatedAt { get; set; }
        }

        public class UploadDocumentRequest
        {
            [JsonPropertyName("typeDocument")]
            public string TypeDocument { get; set; }

            // Le fichier sera géré séparément via MultipartFormDataContent
        }

        // ==================== MEDICAMENTS ====================

        public class Medicament
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("dosage")]
            public string Dosage { get; set; }

            [JsonPropertyName("frequence")]
            public string Frequence { get; set; }

            [JsonPropertyName("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("updatedAt")]
            public DateTime UpdatedAt { get; set; }
        }

        public class CreateMedicamentRequest
        {
            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("dosage")]
            public string Dosage { get; set; }

            [JsonPropertyName("frequence")]
            public string Frequence { get; set; }
        }

        public class UpdateMedicamentRequest
        {
            [JsonPropertyName("nom")]
            public string Nom { get; set; }

            [JsonPropertyName("dosage")]
            public string Dosage { get; set; }

            [JsonPropertyName("frequence")]
            public string Frequence { get; set; }
        }

        // ==================== RENDEZ-VOUS ====================

        public class RendezVous
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("userId")]
            public string UserId { get; set; }

            [JsonPropertyName("date")]
            public DateTime Date { get; set; }

            [JsonPropertyName("heure")]
            public string Heure { get; set; }

            [JsonPropertyName("lieu")]
            public string Lieu { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("createdAt")]
            public DateTime CreatedAt { get; set; }

            [JsonPropertyName("updatedAt")]
            public DateTime UpdatedAt { get; set; }
        }

        public class CreateRendezVousRequest
        {
            [JsonPropertyName("date")]
            public string Date { get; set; }  // Format: "2025-11-30T10:00:00"

            [JsonPropertyName("heure")]
            public string Heure { get; set; }  // Format: "10:00"

            [JsonPropertyName("lieu")]
            public string Lieu { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        public class UpdateRendezVousRequest
        {
            [JsonPropertyName("date")]
            public string Date { get; set; }

            [JsonPropertyName("heure")]
            public string Heure { get; set; }

            [JsonPropertyName("lieu")]
            public string Lieu { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }
        }

        // ==================== RESPONSES ====================

        public class MessageResponse
        {
            [JsonPropertyName("message")]
            public string Message { get; set; }
        }

        public class ErrorResponse
        {
            [JsonPropertyName("error")]
            public string Error { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }
        }
    }
}
