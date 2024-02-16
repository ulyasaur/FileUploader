using FileUploader.Abstractions;
using FileUploader.Constants;
using FileUploader.Models;
using FileUploader.Models.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IBlobStorageUploader _blobStorageUploader;
        private readonly IValidator<UploadFileRequestViewModel> _validator;
        private readonly ILogger<FilesController> _logger;

        public FilesController(
            IBlobStorageUploader blobStorageUploader,
            IValidator<UploadFileRequestViewModel> validator,
            ILogger<FilesController> logger)
        {
            _blobStorageUploader = blobStorageUploader;
            _validator = validator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequestViewModel request)
        {
            ValidationResult validationResult = _validator.Validate(request);

            if (!validationResult.IsValid) 
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                using Stream fileStream = request.Document.OpenReadStream();

                var uploadFileToBlobRequest = new UploadFileToBlobRequest
                {
                    FileName = request.Document.FileName,
                    File = fileStream,
                    Metadata = new Dictionary<string, string>
                    {
                        { MetadataNames.Email, request.Email },
                    },
                };

                await _blobStorageUploader.UploadFileAsync(uploadFileToBlobRequest);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The file could not be uploaded.");

                return Problem();
            }
        }
    }
}