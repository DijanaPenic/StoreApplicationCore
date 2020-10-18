using Microsoft.Extensions.DependencyInjection;

using Store.Cache.Common;
using Store.Cache.Common.Providers;
using Store.Cache.Providers;

namespace Store.Cache.DependencyInjection
{
    public static class CacheExtensions
    {
        public static void AddCacheComponents(this IServiceCollection services, string configuration)
        {
            services.AddTransient<ICacheManager, CacheManager>();
            services.AddTransient<ICacheProviderFactory, RedisCacheProviderFactory>();
            services.AddTransient<IRedisCacheProvider>(provider => new RedisCacheProvider(configuration));
        }
    }
}