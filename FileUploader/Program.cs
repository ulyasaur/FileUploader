using Azure.Identity;
using FileUploader.Abstractions;
using FileUploader.Models.ViewModels;
using FileUploader.Services;
using FileUploader.Services.Validators;
using FluentValidation;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IValidator<UploadFileRequestViewModel>, FileUploadValidator>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(
        new Uri(builder.Configuration.GetConnectionString("Storage")!),
        sharedKeyCredential: new(builder.Configuration.GetSection("Keys")["AccountName"], builder.Configuration.GetSection("Keys")["AccountKey"]));
    clientBuilder.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddScoped<IBlobStorageUploader, BlobStorageUploader>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("DefaultPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
