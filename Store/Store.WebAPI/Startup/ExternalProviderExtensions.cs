using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Models;

namespace Store.WebAPI.Application.Startup
{
    public static class ExternalProviderExtensions
    {
        public static void ConfigureExternalProviders(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieve external login authentication configuration
            IConfigurationSection externalLoginAuthConfig = configuration.GetSection("ExternalLoginAuthentication");

            // Google configuration
            ExternalLoginConfig googleConfig = externalLoginAuthConfig.GetValue<ExternalLoginConfig>("Google");
            if (googleConfig?.ClientId != null)
            {
                services.AddAuthentication().AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                });
            }
        }
    }
}