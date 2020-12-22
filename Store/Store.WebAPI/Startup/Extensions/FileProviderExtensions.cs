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
            services.AddScoped<IFileProvider, LocalFileProvider>(client => 
            {
                string connectionString = configuration.GetConnectionString("Storage");
                return new LocalFileProvider(connectionString);
            });
        }
    }
}
