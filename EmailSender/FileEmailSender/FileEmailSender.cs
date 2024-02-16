using Azure;
using Azure.Communication.Email;
using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileEmailSender
{
    public class FileEmailSender
    {
        private readonly EmailClient _emailClient;

        public FileEmailSender(EmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        [FunctionName("FileEmailSender")]
        public void Run(
            [BlobTrigger("files/{name}.docx", Connection = "FileEmailSender")] Stream myBlob,
            string name,
            Uri uri,
            IDictionary<string, string> metaData,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = "files",
                BlobName = $"{name}.docx",
                ExpiresOn = DateTime.UtcNow.AddHours(1),
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            string accountName = GetEnvironmentVariable("AccountName");
            string accountKey = GetEnvironmentVariable("AccountKey");

            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(accountName, accountKey)).ToString();
            var sasUrl = uri.AbsoluteUri + "?" + sasToken;

            string emailContent = $"The file you have just uploaded: {sasUrl}. Keep in mind, that it will be available only for 1 hour!";

            bool hasRecipientAddress = metaData.TryGetValue(MetadataNames.Email, out string recipientAddress);

            if (!hasRecipientAddress)
            {
                throw new ArgumentException("Recipient email address was not provided in file metadata.");
            }

            EmailSendOperation emailSendOperation = _emailClient.Send(
                WaitUntil.Completed,
                senderAddress: GetEnvironmentVariable("Sender"),
                recipientAddress: recipientAddress,
                subject: "Your document is uploaded!",
                htmlContent: $"<html><h1>Your document is uploaded!</h1><strong>{emailContent}</strong></html>",
                plainTextContent: emailContent);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
