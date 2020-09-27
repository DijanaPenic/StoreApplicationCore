namespace Store.Cache.Common
{
    public interface ICacheManager
    {
        ICacheProvider CacheProvider { get; }
    }
}