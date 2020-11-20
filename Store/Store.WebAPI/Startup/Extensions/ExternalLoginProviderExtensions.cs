using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Models;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class ExternalLoginProviderExtensions
    {
        public static void ConfigureExternalProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
            .AddGoogle(options =>
            {
                ExternalLoginConfig googleConfig = configuration.GetSection("ExternalLoginAuthentication:Google").Get<ExternalLoginConfig>();

                options.ClientId = googleConfig.ClientId;
                options.ClientSecret = googleConfig.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme; 
            });
        }
    }
}