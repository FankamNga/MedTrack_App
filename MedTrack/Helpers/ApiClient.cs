using MedTrack.Constants;
using Org.Apache.Http.Client;
using System;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static MedTrack.Models.ApiModels;


namespace MedTrack.Helpers
{
    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        


        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException() : base("Non autorisé", 401) { }
    }
    internal class ApiClient
    {
        private static readonly Lazy<HttpClient> _httpClientLazy = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(ApiConstants.BaseUrl),
                Timeout = TimeSpan.FromSeconds(ApiConstants.HttpTimeout)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });

        private static HttpClient HttpClient => _httpClientLazy.Value;

        public static async Task<T> GetAsync<T>(string endpoint)
        {
            await AddAuthorizationHeaderAsync();

            var response = await HttpClient.GetAsync(endpoint);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    await AddAuthorizationHeaderAsync();
                    response = await HttpClient.GetAsync(endpoint);
                }
                else
                {
                    throw new UnauthorizedException();
                }
            }

            await EnsureSuccessAsync(response);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static async Task<T> PostAsync<T>(string endpoint, object data, bool requiresAuth = true)
        {
            try
            {

                System.Diagnostics.Debug.WriteLine($"=== POST REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"Endpoint: {HttpClient.BaseAddress}{endpoint}");
                System.Diagnostics.Debug.WriteLine($"RequiresAuth: {requiresAuth}");
                
                if (requiresAuth)
                    await AddAuthorizationHeaderAsync();
                else
                    HttpClient.DefaultRequestHeaders.Authorization = null;

                var json = JsonSerializer.Serialize(data);
                System.Diagnostics.Debug.WriteLine($"Request Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine("Réponse reçue : " + content);
                var response = await HttpClient.PostAsync(endpoint, content);

                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Body: {responseContent}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var refreshed = await TryRefreshTokenAsync();
                    if (refreshed)
                    {
                        await AddAuthorizationHeaderAsync();
                        response = await HttpClient.PostAsync(endpoint, content);
                    }
                    else
                    {
                        throw new UnauthorizedException();
                    }
                }

                await EnsureSuccessAsync(response);
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERROR ===");
                System.Diagnostics.Debug.WriteLine($"Exception Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        public static async Task<T> PutAsync<T>(string endpoint, object data)
        {
            await AddAuthorizationHeaderAsync();

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PutAsync(endpoint, content);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    await AddAuthorizationHeaderAsync();
                    response = await HttpClient.PutAsync(endpoint, content);
                }
                else
                {
                    throw new UnauthorizedException();
                }
            }

            await EnsureSuccessAsync(response);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static async Task<T> DeleteAsync<T>(string endpoint)
        {
            await AddAuthorizationHeaderAsync();

            var response = await HttpClient.DeleteAsync(endpoint);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    await AddAuthorizationHeaderAsync();
                    response = await HttpClient.DeleteAsync(endpoint);
                }
                else
                {
                    throw new UnauthorizedException();
                }
            }

            await EnsureSuccessAsync(response);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static async Task<T> PostFileAsync<T>(string endpoint, byte[] fileBytes, string fileName, string fileType, string documentType = null)
        {
            Console.WriteLine($"[UPLOAD] Début upload → {endpoint}");
            Console.WriteLine($"[UPLOAD] Fichier: {fileName}, Type: {fileType}, Taille: {fileBytes?.Length}");

            await AddAuthorizationHeaderAsync();

            using var formContent = new MultipartFormDataContent();

            // Ajout du fichier
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileType);
            formContent.Add(fileContent, "file", fileName);

            // ⚠️ AJOUT DU TYPE DE DOCUMENT (requis par le backend)
            if (!string.IsNullOrEmpty(documentType))
            {
                formContent.Add(new StringContent(documentType), "typeDocument");
            }

            var response = await HttpClient.PostAsync(endpoint, formContent);
            Console.WriteLine($"[UPLOAD] StatusCode → {response.StatusCode}");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("[UPLOAD] Token expired → trying refresh");
                var refreshed = await TryRefreshTokenAsync();

                if (refreshed)
                {
                    // ⚠️ IMPORTANT: Recréer le formContent car il a déjà été utilisé
                    using var newFormContent = new MultipartFormDataContent();
                    var newFileContent = new ByteArrayContent(fileBytes);
                    newFileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileType);
                    newFormContent.Add(newFileContent, "file", fileName);

                    if (!string.IsNullOrEmpty(documentType))
                    {
                        newFormContent.Add(new StringContent(documentType), "typeDocument");
                    }

                    await AddAuthorizationHeaderAsync();
                    response = await HttpClient.PostAsync(endpoint, newFormContent);
                    Console.WriteLine($"[UPLOAD] StatusCode after refresh → {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine("[UPLOAD] Refresh token failed");
                    throw new UnauthorizedException();
                }
            }

            var rawResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[UPLOAD] Raw Response → {rawResponse}");

            await EnsureSuccessAsync(response);

            return JsonSerializer.Deserialize<T>(rawResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        private static async Task AddAuthorizationHeaderAsync()
        {
            var token = await TokenManager.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private static async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var refreshToken = await TokenManager.GetRefreshTokenAsync();
                if (string.IsNullOrEmpty(refreshToken)) return false;

                HttpClient.DefaultRequestHeaders.Authorization = null;

                var response = await HttpClient.PostAsync(ApiConstants.RefreshTokenEndpoint,
                    new StringContent(JsonSerializer.Serialize(new { RefreshToken = refreshToken }), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode) return false;

                var content = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await TokenManager.SaveAccessTokenAsync(authResponse.Session.AccessToken);
                await TokenManager.SaveRefreshTokenAsync(authResponse.Session.RefreshToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var content = await response.Content.ReadAsStringAsync();
            ErrorResponse error = null;

            try
            {
                error = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch { }

            var message = error?.Message ?? error?.Error ?? "Une erreur est survenue";
            throw new ApiException(message, (int)response.StatusCode);
        }
    }
    }
