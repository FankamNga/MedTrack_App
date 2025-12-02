using MedTrack.Constants;
using MedTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> SignupAsync(SignupRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }

    public class AuthService : IAuthService
    {
        public async Task<AuthResponse> SignupAsync(SignupRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"=== POST REQUEST ===");
            System.Diagnostics.Debug.WriteLine($"[AuthService] Email: {request.Email}");
            System.Diagnostics.Debug.WriteLine($"[AuthService] Password Length: {request.Password?.Length}");

            var response = await ApiClient.PostAsync<AuthResponse>(ApiConstants.SignupEndpoint, request, requiresAuth: false);

            // Sauvegarder les tokens automatiquement
            await TokenManager.SaveAccessTokenAsync(response.Session.AccessToken);
            await TokenManager.SaveRefreshTokenAsync(response.Session.RefreshToken);

            return response;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var response = await ApiClient.PostAsync<AuthResponse>(ApiConstants.LoginEndpoint, request, true);

            // Sauvegarder les tokens automatiquement
            await TokenManager.SaveAccessTokenAsync(response.Session.AccessToken);
            await TokenManager.SaveRefreshTokenAsync(response.Session.RefreshToken);

            return response;
        }

        public async Task LogoutAsync()
        {
            TokenManager.ClearTokens();
            await Task.CompletedTask;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await TokenManager.IsAuthenticatedAsync();
        }
    }
}
