using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Store.FileProvider.Common.Core;

namespace Store.FileProvider.Providers
{
    public class AzureBlobFileProvider : IFileProvider
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobFileProvider(string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            _blobClient = account.CreateCloudBlobClient();
        }

        public async Task<bool> DeleteFileAsync(string storageName, string filePath)
        {
            return GetBlockBlob(storageName, filePath).DeleteIfExistsAsync();
        }

        public Task<bool> FileExistsAsync(string storageName, string filePath)
        {
            return GetBlockBlob(storageName, filePath).ExistsAsync();
        }

        public async Task<Stream> GetFileAsync(string storageName, string filePath)
        {
            CloudBlockBlob blob = GetBlockBlob(storageName, filePath);

            MemoryStream memory = new MemoryStream();
            await blob.DownloadToStreamAsync(memory).ConfigureAwait(false);
            memory.Seek(0, SeekOrigin.Begin);

            return memory;
        }

        public async Task<string> GetFileUrlAsync(string storageName, string filePath)
        {
            CloudBlockBlob blob = GetBlockBlob(storageName, filePath);
            string url = null;

            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                url = blob.Uri.AbsoluteUri;
            }

            return url;
        }

        public Task SaveFileAsync(string storageName, string filePath, Stream fileStream)
        {
            return GetBlockBlob(storageName, filePath).UploadFromStreamAsync(fileStream);
        }

        private CloudBlockBlob GetBlockBlob(string storageName, string filePath)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(storageName);
            CloudBlockBlob blob = container.GetBlockBlobReference(filePath.ToLower());

            return blob;
        }
    }
}