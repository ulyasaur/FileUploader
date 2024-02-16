namespace FileUploader.Models
{
    public class UploadFileToBlobRequest
    {
        public Stream File { get; set; }

        public string FileName { get; set; }

        public IDictionary<string, string> Metadata { get; set; }
    }
}
