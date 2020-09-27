using System;
using System.Threading.Tasks;

namespace Store.Cache.Common
{
    public interface ICacheProvider
    {
        string KeyPrefix { get; set; }

        T Get<T>(string key, string group = null);

        bool Contains(string key, string group = null);

        bool Remove(string key, string group = null);

        bool Add<T>(string key, T value, DateTimeOffset absoluteExpiration, string group = null);

        T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration, string group = null);

        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> valueFactory, DateTimeOffset absoluteExpiration, string group = null);
    }
}