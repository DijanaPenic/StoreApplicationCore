using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Infrastructure.Authorization;
using Store.WebAPI.Infrastructure.Authorization.Handlers;
using Store.WebAPI.Infrastructure.Authorization.Providers;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            // Authorization policy provider
            services.AddTransient<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            // Handlers must be provided for the requirements of the authorization policies
            services.AddTransient<IAuthorizationHandler, SectionAuthorizationHandler>();

            // Permission builder
            services.AddTransient<IPermissionsBuilder, PermissionsBuilder>();
        }
    }
}