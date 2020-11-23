using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Infrastructure.Models;

namespace Store.WebAPI.Application.Startup.Extensions
{
    // TODO - need to move external provider configuration to web application

    // Redirect URL:
    // * Google - must end with public domain, such as "com", "org". Otherwise, localhost must be used.
    // * Facebook - must be "https"
    public static class ExternalLoginProviderExtensions
    {
        public static void ConfigureExternalProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
            .AddGoogle(GetExternalLoginAuthOptions(configuration, "Google"))
            .AddFacebook(GetExternalLoginAuthOptions(configuration, "Facebook"));
        }

        static Action<OAuthOptions> GetExternalLoginAuthOptions(IConfiguration configuration, string providerName)
        {
            ExternalLoginConfig config = configuration.GetSection($"ExternalLoginAuthentication:{providerName}").Get<ExternalLoginConfig>();

            void externalLoginAuthOptions(OAuthOptions options)
            {

                options.ClientId = config.ClientId;
                options.ClientSecret = config.ClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            }

            return externalLoginAuthOptions;
        }
    }
}