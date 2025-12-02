using MedTrack.Constants;
using MedTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Services
{
    public interface IRendezVousService
    {
        Task<RendezVous> CreateRendezVousAsync(CreateRendezVousRequest request);
        Task<List<RendezVous>> GetRendezVousAsync();
        Task<RendezVous> UpdateRendezVousAsync(string id, UpdateRendezVousRequest request);
        Task<MessageResponse> DeleteRendezVousAsync(string id);
    }

    public class RendezVousService : IRendezVousService
    {
        public async Task<RendezVous> CreateRendezVousAsync(CreateRendezVousRequest request)
        {
            return await ApiClient.PostAsync<RendezVous>(ApiConstants.RendezVousEndpoint, request);
        }

        public async Task<List<RendezVous>> GetRendezVousAsync()
        {
            return await ApiClient.GetAsync<List<RendezVous>>(ApiConstants.RendezVousEndpoint);
        }

        public async Task<RendezVous> UpdateRendezVousAsync(string id, UpdateRendezVousRequest request)
        {
            return await ApiClient.PutAsync<RendezVous>($"{ApiConstants.RendezVousEndpoint}/{id}", request);
        }

        public async Task<MessageResponse> DeleteRendezVousAsync(string id)
        {
            return await ApiClient.DeleteAsync<MessageResponse>($"{ApiConstants.RendezVousEndpoint}/{id}");
        }
    }
}
