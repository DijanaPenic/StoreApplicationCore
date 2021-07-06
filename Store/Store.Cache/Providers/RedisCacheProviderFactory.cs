using Store.Cache.Common;
using Store.Cache.Common.Providers;

namespace Store.Cache.Providers
{
    public class RedisCacheProviderFactory : ICacheProviderFactory
    {
        private readonly IRedisCacheProvider _redisCacheProvider;

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