using MedTrack.Constants;
using MedTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Services
{
    public interface IMedicamentsService
    {
        Task<Medicament> CreateMedicamentAsync(CreateMedicamentRequest request);
        Task<List<Medicament>> GetMedicamentsAsync();
        Task<Medicament> UpdateMedicamentAsync(string id, UpdateMedicamentRequest request);
        Task<MessageResponse> DeleteMedicamentAsync(string id);
    }
    public class MedicamentsService : IMedicamentsService
    {
        public async Task<Medicament> CreateMedicamentAsync(CreateMedicamentRequest request)
        {
            return await ApiClient.PostAsync<Medicament>(ApiConstants.MedicamentsEndpoint, request);
        }

        public async Task<List<Medicament>> GetMedicamentsAsync()
        {
            return await ApiClient.GetAsync<List<Medicament>>(ApiConstants.MedicamentsEndpoint);
        }

        public async Task<Medicament> UpdateMedicamentAsync(string id, UpdateMedicamentRequest request)
        {
            return await ApiClient.PutAsync<Medicament>($"{ApiConstants.MedicamentsEndpoint}/{id}", request);
        }

        public async Task<MessageResponse> DeleteMedicamentAsync(string id)
        {
            return await ApiClient.DeleteAsync<MessageResponse>($"{ApiConstants.MedicamentsEndpoint}/{id}");
        }
    }
}
