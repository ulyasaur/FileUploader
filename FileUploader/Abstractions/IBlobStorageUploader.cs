using FileUploader.Models;

namespace FileUploader.Abstractions
{
    public interface IBlobStorageUploader
    {
        Task UploadFileAsync(UploadFileToBlobRequest request);
    }
}
