using FileUploader.Abstractions;
using FileUploader.Controllers;
using FileUploader.Models;
using FileUploader.Models.ViewModels;
using FluentAssertions;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FileUploader.Tests
{
    public class FilesControllerTests
    {
        private const int INTERNAL_SERVER_ERROR = 500;

        private readonly FilesController _filesController;

        private readonly Mock<ILogger<FilesController>> _loggerMock = new();
        private readonly Mock<IBlobStorageUploader> _blobStorageUploaderMock = new();
        private readonly Mock<IValidator<UploadFileRequestViewModel>> _validatorMock = new();

        private readonly UploadFileRequestViewModel _uploadFileRequestViewModel;

        public FilesControllerTests()
        {
            ServiceCollection serviceCollection = new();

            serviceCollection
                .AddSingleton(_loggerMock.Object)
                .AddSingleton(_blobStorageUploaderMock.Object)
                .AddSingleton(_validatorMock.Object)
                .AddSingleton<FilesController>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            _filesController = serviceProvider.GetRequiredService<FilesController>();

            _uploadFileRequestViewModel = new()
            {
                Email = "test@gmail.com",
                Document = Mock.Of<IFormFile>(),
            };
        }

        [Fact]
        public async Task UploadFile_Returns500InternalServerError_UnknownIssues()
        {
            // Arrange
            _blobStorageUploaderMock.Setup(q =>
                q.UploadFileAsync(
                    It.IsAny<UploadFileToBlobRequest>()))
                .Throws(new Exception());

            ValidationResult validationResult = Mock.Of<ValidationResult>(r => r.IsValid == true);

            _validatorMock.Setup(q =>
                q.Validate(It.IsAny<UploadFileRequestViewModel>()))
                .Returns(validationResult);

            // Act
            IActionResult result = await _filesController.UploadFile(_uploadFileRequestViewModel);

            // Assert
            _blobStorageUploaderMock.Verify(
                u => u.UploadFileAsync(
                    It.IsAny<UploadFileToBlobRequest>()),
                Times.AtLeastOnce);

            result.As<ObjectResult>().StatusCode.Should().Be(INTERNAL_SERVER_ERROR);
        }

        [Fact]
        public async Task UploadFile_Returns200Ok()
        {
            // Arrange
            _blobStorageUploaderMock.Setup(q =>
                q.UploadFileAsync(
                    It.IsAny<UploadFileToBlobRequest>()));

            ValidationResult validationResult = Mock.Of<ValidationResult>(r => r.IsValid == true);

            _validatorMock.Setup(q =>
                q.Validate(It.IsAny<UploadFileRequestViewModel>()))
                .Returns(validationResult);

            // Act
            IActionResult result = await _filesController.UploadFile(_uploadFileRequestViewModel);

            // Assert
            _blobStorageUploaderMock.Verify(
                u => u.UploadFileAsync(
                    It.IsAny<UploadFileToBlobRequest>()),
                Times.AtLeastOnce);

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task UploadFile_Returns400BadRequest()
        {
            // Arrange
            UploadFileRequestViewModel request = new()
            {
                Email = "wrongEmail",
                Document = Mock.Of<IFormFile>(f => f.ContentType == "wrong-type"),
            };

            ValidationResult validationResult = Mock.Of<ValidationResult>(r => r.IsValid == false);

            _validatorMock.Setup(q =>
                q.Validate(It.IsAny<UploadFileRequestViewModel>()))
                .Returns(validationResult);

            // Act

            IActionResult result = await _filesController.UploadFile(request);

            // Assert
            _blobStorageUploaderMock.Verify(
                u => u.UploadFileAsync(
                    It.IsAny<UploadFileToBlobRequest>()),
                Times.Never);

            result.As<ObjectResult>().StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
    }
}