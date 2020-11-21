using System;

using Store.Cache.Common;
using Store.Cache.Common.Providers;

namespace Store.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly Lazy<ICacheProvider> _cacheProvider;
        private readonly ICacheProviderFactory _cacheProviderFactory;

        public ICacheProvider CacheProvider
        {
            get
            {
                return _cacheProvider.Value;
            }
        }

        public CacheManager(ICacheProviderFactory cacheProviderFactory)
        {
            _cacheProviderFactory = cacheProviderFactory;
            _cacheProvider = new Lazy<ICacheProvider>(() => ResolveProvider(), true);
        }

        private ICacheProvider ResolveProvider()
        {
            ICacheProvider cacheProvider = _cacheProviderFactory.CreateCacheProvider();
            cacheProvider.KeyPrefix = "Store";

            return cacheProvider;
        }
    }
}