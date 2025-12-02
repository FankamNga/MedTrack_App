using MedTrack.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedTrack.Helpers
{
    internal class TokenManager
    {
        public static async Task SaveAccessTokenAsync(string token)
        {
            await SecureStorage.SetAsync(ApiConstants.AccessTokenKey, token);
        }

        public static async Task SaveRefreshTokenAsync(string token)
        {
            await SecureStorage.SetAsync(ApiConstants.RefreshTokenKey, token);
        }

        public static async Task<string> GetAccessTokenAsync()
        {
            return await SecureStorage.GetAsync(ApiConstants.AccessTokenKey);
        }

        public static async Task<string> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync(ApiConstants.RefreshTokenKey);
        }

        public static void ClearTokens()
        {
            SecureStorage.Remove(ApiConstants.AccessTokenKey);
            SecureStorage.Remove(ApiConstants.RefreshTokenKey);
        }

        public static async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetAccessTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
