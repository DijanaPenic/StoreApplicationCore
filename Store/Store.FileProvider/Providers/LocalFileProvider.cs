using System.IO;
using System.Threading.Tasks;

using Store.FileProvider.Common.Core;

namespace Store.FileProvider.Providers
{
    public class LocalFileProvider : IFileProvider
    {
        private readonly string _fileRoot;

        public LocalFileProvider(string fileRoot)
        {
            _fileRoot = fileRoot;
        }

        public Task<bool> DeleteFileAsync(string storageName, string filePath)
        {
            string path = Path.Combine(_fileRoot, storageName, filePath);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.FromResult(true);
        }

        public Task<bool> FileExistsAsync(string storageName, string filePath)
        {
            string path = Path.Combine(_fileRoot, storageName, filePath);

            return Task.FromResult(File.Exists(path));
        }

        public Task<Stream> GetFileAsync(string storageName, string filePath)
        {
            string path = Path.Combine(_fileRoot, storageName, filePath);
            Stream stream = null;

            if (File.Exists(path))
            {
                stream = File.OpenRead(path);
            }

            return Task.FromResult(stream);
        }

        public Task<string> GetFileUrlAsync(string storageName, string filePath)
        {
            return Task.FromResult((string)null);
        }

        public async Task<string> SaveFileAsync(string storageName, string filePath, Stream fileStream)
        {
            filePath = Path.Combine(_fileRoot, storageName, filePath);
            string dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using FileStream file = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(file).ConfigureAwait(false);

            return filePath;
        }
    }
}