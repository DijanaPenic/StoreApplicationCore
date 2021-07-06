using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.WebAPI.Infrastructure.Authorization.Handlers;
using Store.WebAPI.Infrastructure.Authorization.Providers;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorizationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Authorization policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            // Handlers must be provided for the requirements of the authorization policies
            services.AddScoped<IAuthorizationHandler, SectionAuthorizationHandler>();
        }
    }
}