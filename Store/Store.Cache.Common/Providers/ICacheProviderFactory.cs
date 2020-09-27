namespace Store.Cache.Common.Providers
{
    public interface ICacheProviderFactory
    {
        ICacheProvider CreateCacheProvider();
    }
}