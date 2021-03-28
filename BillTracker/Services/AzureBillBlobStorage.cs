using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using BillTracker.Modules;

namespace BillTracker.Services
{
    internal class AzureBillBlobStorage : IBillFileStorage, IInitializable
    {
        private const string ContainerName = "bills";

        private readonly AzureConfiguration _configuration;

        public AzureBillBlobStorage(AzureConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize()
        {
            var containerClient = new BlobContainerClient(
                _configuration.BlobStorageConnectionString,
                ContainerName,
                new BlobClientOptions(BlobClientOptions.ServiceVersion.V2019_12_12));
            containerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }

        public async Task<Uri> Upload(UploadBillBlob parameters)
        {
            parameters.File.Seek(0, SeekOrigin.Begin);
            var blobClient = new BlobClient(
                _configuration.BlobStorageConnectionString,
                ContainerName,
                GetBlobName(parameters),
                new BlobClientOptions(BlobClientOptions.ServiceVersion.V2019_12_12));

            await blobClient.UploadAsync(parameters.File);
            return blobClient.Uri;
        }

        private static string GetBlobName(UploadBillBlob parameters) =>
            Path.Combine(
                parameters.UserId.ToString(),
                parameters.AggregateId.ToString(),
                parameters.FileName);
    }
}
