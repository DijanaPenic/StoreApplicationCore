using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

using Store.FileProvider.Common.Core;

namespace Store.FileProvider.Providers
{
    public class AzureBlobFileProvider : IFileProvider
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobFileProvider(string connectionString)
        {
            // Create a BlobServiceClient object which will be used to create a container client
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<bool> DeleteFileAsync(string storageName, string filePath)
        {
            BlockBlobClient blob = await GetBlockBlobAsync(storageName, filePath);

            return await blob.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string storageName, string filePath)
        {
            BlockBlobClient blob = await GetBlockBlobAsync(storageName, filePath);

            return await blob.ExistsAsync();
        }

        public async Task<Stream> GetFileAsync(string storageName, string filePath)
        {
            BlockBlobClient blob = await GetBlockBlobAsync(storageName, filePath);

            MemoryStream memory = new MemoryStream();
            await blob.DownloadToAsync(memory).ConfigureAwait(false);
            memory.Seek(0, SeekOrigin.Begin);

            return memory;
        }

        public async Task<string> GetFileUrlAsync(string storageName, string filePath)
        {
            BlockBlobClient blob = await GetBlockBlobAsync(storageName, filePath);
            string url = null;

            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                url = blob.Uri.AbsoluteUri;
            }

            return url;
        }

        public async Task<string> SaveFileAsync(string storageName, string filePath, Stream fileStream)
        {
            BlockBlobClient blob = await GetBlockBlobAsync(storageName, filePath);

            await blob.UploadAsync(fileStream);

            return blob.Uri.AbsoluteUri;
        }

        private async Task<BlockBlobClient> GetBlockBlobAsync(string storageName, string filePath)
        {
            // Create the container and return a container client object
            BlobContainerClient containerClient = await _blobServiceClient.CreateBlobContainerAsync(storageName);
            await containerClient.CreateIfNotExistsAsync();

            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(filePath.ToLower());

            return blockBlobClient;
        }
    }
}