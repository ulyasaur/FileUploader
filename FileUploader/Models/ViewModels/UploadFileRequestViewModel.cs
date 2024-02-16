namespace FileUploader.Models.ViewModels
{
    public class UploadFileRequestViewModel
    {
        public string Email { get; set; }

        public IFormFile Document { get; set; }
    }
}
