using Azure.Storage.Blobs;
using FileUploader.Abstractions;
using FileUploader.Models;

namespace FileUploader.Services
{
    public class BlobStorageUploader : IBlobStorageUploader
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageUploader> _logger;

        public BlobStorageUploader(
            BlobServiceClient blobServiceClient, 
            ILogger<BlobStorageUploader> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task UploadFileAsync(UploadFileToBlobRequest request)
        {
            try
            {
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("files");
                BlobClient blobClient = blobContainerClient.GetBlobClient(request.FileName);

                await blobClient.UploadAsync(request.File);
                await blobClient.SetMetadataAsync(request.Metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The file could not be uploaded.");
                throw;
            }
        }
    }
}
