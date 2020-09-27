using Microsoft.Extensions.DependencyInjection;

using Store.Cache.Common;
using Store.Cache.Common.Providers;
using Store.Cache.Providers;

namespace Store.Cache.DependencyInjection
{
    public static class CacheExtensions
    {
        public static void ConfigureCacheComponents(this IServiceCollection services)
        {
            services.AddTransient<ICacheManager, CacheManager>();
            services.AddTransient<ICacheProviderFactory, RedisCacheProviderFactory>();
        }
    }
}