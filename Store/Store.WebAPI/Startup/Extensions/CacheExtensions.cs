using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Store.Cache;
using Store.Cache.Common;
using Store.Cache.Common.Providers;
using Store.Cache.Providers;

namespace Store.WebAPI.Application.Startup.Extensions
{
    public static class CacheExtensions
    {
        public static void AddCacheComponents(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Redis");

            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<ICacheProviderFactory, RedisCacheProviderFactory>();
            services.AddScoped<IRedisCacheProvider>(provider => new RedisCacheProvider(connectionString));
        }
    }
}