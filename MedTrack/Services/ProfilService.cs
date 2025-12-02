using MedTrack.Constants;
using MedTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Services
{
    public interface IProfilService
    {
        Task<Profil> GetProfilAsync();
        Task<Profil> UpdateProfilAsync(UpdateProfilRequest request);
    }

    internal class ProfilService : IProfilService
    {
        public async Task<Profil> GetProfilAsync()
        {
            return await ApiClient.GetAsync<Profil>(ApiConstants.ProfilEndpoint);
        }

        public async Task<Profil> UpdateProfilAsync(UpdateProfilRequest request)
        {
            return await ApiClient.PutAsync<Profil>(ApiConstants.ProfilEndpoint, request);
        }
    }
}
