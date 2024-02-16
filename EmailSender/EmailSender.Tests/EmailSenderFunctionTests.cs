using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FileEmailSender.Tests
{
    public class EmailSenderFunctionTests
    {
        private readonly FileEmailSender _emailSender;

        private string _blobName;
        private readonly Mock<Stream> _blobMock = new();
        private readonly Mock<EmailClient> _emailClientMock = new();
        private readonly Uri _uri = new("https://test.com");
        private IDictionary<string, string> _metadata;
        private readonly Mock<ILogger> _loggerMock = new();

        public EmailSenderFunctionTests()
        {
            ServiceCollection serviceCollection = new();

            serviceCollection
                .AddSingleton(_emailClientMock.Object)
                .AddSingleton<FileEmailSender>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            _emailSender = serviceProvider.GetRequiredService<FileEmailSender>();

            Environment.SetEnvironmentVariable("AccountName", "test");
            Environment.SetEnvironmentVariable("AccountKey", "test");
            Environment.SetEnvironmentVariable("CommunicationService", "test");
        }

        [Fact]
        public async Task EmailSender_Run_ValidParams()
        {
            _blobName = Guid.NewGuid().ToString();
            _metadata = new Dictionary<string, string>
            {
                { MetadataNames.Email, "test@gmail.com" },
            };

            _emailClientMock.Setup(q =>
                q.Send(
                    It.IsAny<WaitUntil>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Mock.Of<EmailSendOperation>(o => o.HasCompleted == true));

            _emailSender.Run(_blobMock.Object, _blobName, _uri, _metadata, _loggerMock.Object);
        }

        [Fact]
        public async Task EmailSender_Run_Throws_ArgumentException_NoRecipient()
        {
            _blobName = Guid.NewGuid().ToString();
            _metadata = new Dictionary<string, string>();

            _emailClientMock.Verify(
                c => c.Send(
                    It.IsAny<WaitUntil>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            Assert.Throws<ArgumentException>(() => _emailSender.Run(_blobMock.Object, _blobName, _uri, _metadata, _loggerMock.Object));
        }
    }
}