using MedTrack.Constants;
using MedTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static MedTrack.Models.ApiModels;

namespace MedTrack.Services
{
    public interface IDocumentsService
        {
            Task<Document> UploadDocumentAsync(byte[] fileBytes, string fileName, string fileType, string documentType);
            Task<List<Document>> GetDocumentsAsync();
            Task<MessageResponse> DeleteDocumentAsync(string id);
        }
    public class DocumentsService : IDocumentsService
    {

        public async Task<Document> UploadDocumentAsync(byte[] fileBytes, string fileName, string fileType, string documentType)
        {
            return await ApiClient.PostFileAsync<Document>(
                ApiConstants.DocumentsEndpoint,
                fileBytes,
                fileName,
                fileType,
                documentType  // ← Nouveau paramètre
            );
        }

        public async Task<List<Document>> GetDocumentsAsync()
            {
                return await ApiClient.GetAsync<List<Document>>(ApiConstants.DocumentsEndpoint);
            }

            public async Task<MessageResponse> DeleteDocumentAsync(string id)
            {
                return await ApiClient.DeleteAsync<MessageResponse>($"{ApiConstants.DocumentsEndpoint}/{id}");
            }
        }

}
