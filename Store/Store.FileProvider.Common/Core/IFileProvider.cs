using System.IO;
using System.Threading.Tasks;

namespace Store.FileProvider.Common.Core
{
    public interface IFileProvider
    {
        Task<bool> DeleteFileAsync(string storageName, string filePath);

        Task<bool> FileExistsAsync(string storageName, string filePath);

        Task<Stream> GetFileAsync(string storageName, string filePath);

        Task<string> GetFileUrlAsync(string storageName, string filePath);

        Task SaveFileAsync(string storageName, string filePath, Stream fileStream);
    }
}