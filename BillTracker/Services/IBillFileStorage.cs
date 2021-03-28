using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using BillTracker.Modules;

namespace BillTracker.Services
{
    public interface IBillFileStorage
    {
        Task<Uri> Upload(Stream file, Guid userId, Guid fileId);
    }

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

        public async Task<Uri> Upload(Stream file, Guid userId, Guid fileId)
        {
            return null;
        }
    }
}
