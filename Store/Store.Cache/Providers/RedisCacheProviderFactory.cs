using Store.Cache.Common;
using Store.Cache.Common.Providers;

namespace Store.Cache.Providers
{
    class RedisCacheProviderFactory : ICacheProviderFactory
    {
        IRedisCacheProvider _redisCacheProvider;

        public RedisCacheProviderFactory(IRedisCacheProvider redisCacheProvider)
        {
            _redisCacheProvider = redisCacheProvider;
        }

        public ICacheProvider CreateCacheProvider()
        {
            return _redisCacheProvider;
        }
    }
}