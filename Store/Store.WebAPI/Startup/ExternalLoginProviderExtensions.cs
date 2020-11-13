using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Models;

namespace Store.WebAPI.Application.Startup
{
    public static class ExternalLoginProviderExtensions
    {
        public static void ConfigureExternalProviders(this IServiceCollection services, IConfiguration configuration)
        {
            // Google configuration
            ExternalLoginConfig googleConfig = configuration.GetSection("ExternalLoginAuthentication:Google").Get<ExternalLoginConfig>();
            if (googleConfig?.ClientId != null)
            {
                services.AddAuthentication().AddGoogle(options =>
                {
                    options.ClientId = googleConfig.ClientId;
                    options.ClientSecret = googleConfig.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme; 
                });
            }
        }
    }
}