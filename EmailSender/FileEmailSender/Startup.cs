using Azure.Communication.Email;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(FileEmailSender.Startup))]

namespace FileEmailSender
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<EmailClient>(c => new EmailClient(Environment.GetEnvironmentVariable("CommunicationService")));
        }
    }
}
