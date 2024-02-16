using FileUploader.Constants;
using FileUploader.Models.ViewModels;
using FluentValidation;

namespace FileUploader.Services.Validators
{
    public class FileUploadValidator : AbstractValidator<UploadFileRequestViewModel>
    {
        public FileUploadValidator() 
        { 
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
            RuleFor(r => r.Document).Must(d => d.ContentType == FileExtensions.Docx);
        }
    }
}
