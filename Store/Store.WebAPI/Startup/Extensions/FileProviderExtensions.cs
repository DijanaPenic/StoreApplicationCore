using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.FileProvider.Providers;
using Store.FileProvider.Common.Core;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class FileProviderExtensions
    {
        public static void AddFileProviderServices(this IServiceCollection services, IConfiguration configuration)
        {
            bool isCloud = configuration.GetValue<bool>("IsCloud");
            string connectionString = configuration.GetConnectionString("Storage");

            if (!isCloud)
            {
                services.AddScoped<IFileProvider, LocalFileProvider>(client => new LocalFileProvider(connectionString));
            }
            else
            {
                services.AddScoped<IFileProvider, AzureBlobFileProvider>(client => new AzureBlobFileProvider(connectionString));
            }
        }
    }
}
