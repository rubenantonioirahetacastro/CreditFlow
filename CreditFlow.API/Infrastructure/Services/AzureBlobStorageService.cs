using Azure.Storage.Blobs;
using CreditFlow.API.Core.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CreditFlow.API.Infrastructure.Services
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        // La conexión se crea al primer uso: si falta la configuración, solo fallan las operaciones de archivos
        // y no los controladores que reciben este servicio (p. ej. el listado de empleados).
        private readonly Lazy<BlobContainerClient> _container;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            _container = new Lazy<BlobContainerClient>(() =>
            {
                var connectionString = configuration["AzureBlobStorageConnectionString"]
                    ?? throw new InvalidOperationException("No se encontró 'AzureBlobStorageConnectionString' en la configuración.");

                var containerName = configuration["AzureBlobStorageContainerName"] ?? "documentos";

                var containerClient = new BlobServiceClient(connectionString).GetBlobContainerClient(containerName);
                containerClient.CreateIfNotExists();
                return containerClient;
            });
        }

        private BlobContainerClient _containerClient => _container.Value;

        public async Task<string> UploadImageAsync(Stream fileStream, string folder, string fileName)
        {
            if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            var normalizedFolder = string.IsNullOrWhiteSpace(folder)
                ? string.Empty
                : folder.Trim('/');

            var blobName = string.IsNullOrEmpty(normalizedFolder)
                ? fileName
                : $"{normalizedFolder}/{fileName}";

            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobName;
        }

        public async Task<Stream?> DownloadImageAsync(string blobPath)
        {
            if (string.IsNullOrWhiteSpace(blobPath)) return null;

            var blobClient = _containerClient.GetBlobClient(blobPath);
            if (!await blobClient.ExistsAsync())
                return null;

            return await blobClient.OpenReadAsync();
        }
    }
}
