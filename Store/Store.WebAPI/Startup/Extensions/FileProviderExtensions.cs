using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.FileProvider.Providers;
using Store.FileProvider.Common.Core;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class FileProviderExtensions
    {
        public static void AddFileProvider(this IServiceCollection services, IConfiguration configuration)
        {
            bool localStorageEnabled = configuration.GetValue<bool>("LocalStorageEnabled");

            if (localStorageEnabled)
            {
                services.AddScoped<IFileProvider, LocalFileProvider>(client =>
                {
                    string connectionString = configuration.GetConnectionString("LocalStorage");
                    return new LocalFileProvider(connectionString);
                });
            }
            else
            {
                services.AddScoped<IFileProvider, AzureBlobFileProvider>(client =>
                {
                    string connectionString = configuration.GetConnectionString("AzureStorage");
                    return new AzureBlobFileProvider(connectionString);
                });
            }
        }
    }
}
