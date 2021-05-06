using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Files;

namespace SIO.Infrastructure.Azure.Storage
{
    internal sealed class AzureFileClient : IFileClient
    {
        private readonly AzureStorageBlobOptions _options;

        public AzureFileClient(IOptions<AzureStorageBlobOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Value;
        }

        public async Task DeleteAsync(string fileName, string userId, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobAsync(fileName, userId);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DownloadAsync(string fileName, string userId, Stream stream, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobAsync(fileName, userId);
            await blobClient.DownloadToAsync(stream);
        }

        public async Task UploadAsync(string fileName, string userId, Stream stream, CancellationToken cancellationToken = default)
        {
            var blobClient = await GetBlobAsync(fileName, userId);
            await blobClient.UploadAsync(stream);
        }

        private async Task<BlobClient> GetBlobAsync(string fileName, string userId)
        {
            BlobContainerClient container = new BlobContainerClient(_options.ConnectionString, userId);
            await container.CreateIfNotExistsAsync();
            return container.GetBlobClient(fileName);
        }
    }
}
