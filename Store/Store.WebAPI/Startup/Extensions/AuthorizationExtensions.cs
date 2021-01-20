using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientAuthenticationPolicy", new AuthorizationPolicyBuilder("ClientAuthenticationScheme").RequireAuthenticatedUser().Build()); 
            });
        }
    }
}