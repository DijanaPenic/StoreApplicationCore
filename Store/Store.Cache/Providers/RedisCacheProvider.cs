using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using StackExchange.Redis;

using Store.Model.JsonConverters;
using Store.Cache.Common.Providers;

namespace Store.Cache.Providers
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private static readonly object padLock = new object();
        private static readonly object padLockDatabase = new object();
        private static volatile IDatabase _database = null; // volatile - variable must never be cached
        private readonly string _configuration; 

        public string KeyPrefix { get; set; }

        public virtual IDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    lock (padLockDatabase)
                    {
                        ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(_configuration);
                        _database = connection.GetDatabase();
                    }
                }

                return _database;
            }
            private set
            {
                _database = value;
            }
        }

        public RedisCacheProvider(string configuration)
        {
            _configuration = configuration;
        }

        public T Get<T>(string key, string group = null)
        {
            key = BuildKey(key, group);
            RedisValue obj = default;

            try
            {
                obj = Database.StringGet(key);
            }
            catch (InvalidCastException) { }
            catch (InvalidOperationException) { }
            catch (RedisConnectionException) { }

            if (obj.HasValue)
                return JsonConvert.DeserializeObject<T>(obj.ToString(), GetJsonSerializerSettings());
            else
                return default;
        }

        public bool Contains(string key, string group = null)
        {
            return Get<object>(key, group) != null;
        }

        public bool Remove(string key, string group = null)
        {
            lock (padLock)
            {
                try
                {
                    return Database.KeyDelete(BuildKey(key, group));
                }
                catch (InvalidCastException) { }
                catch (InvalidOperationException) { }
                catch (RedisConnectionException) { }
            }

            return false;
        }

        public bool Add<T>(string key, T value, DateTimeOffset absoluteExpiration, string group = null)
        {
            if (value != null)
            {
                lock (padLock)
                {
                    try
                    {
                        key = BuildKey(key, group);

                        // Max expiration can be set to 365 days from now.
                        DateTimeOffset maxExpiration = DateTimeOffset.UtcNow.AddDays(365);
                        string obj = JsonConvert.SerializeObject(value, Formatting.Indented, GetJsonSerializerSettings());

                        if (absoluteExpiration != null && absoluteExpiration < maxExpiration)
                        {
                            TimeSpan expiry = absoluteExpiration - DateTime.UtcNow;
                            return Database.StringSet(key, obj, expiry);
                        }
                        else
                        {
                            return Database.StringSet(key, obj);
                        }
                    }
                    catch (InvalidCastException) { }
                    catch (InvalidOperationException) { }
                    catch (RedisConnectionException) { }
                }
            }

            return false;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> valueFactory, DateTimeOffset absoluteExpiration, string group = null)
        {
            T result = Get<T>(key, group);

            if (result == null)
            {
                result = await valueFactory.Invoke();

                if (result != null)
                {
                    Add(key, result, absoluteExpiration, group);
                }
            }

            return result;
        }

        public T GetOrAdd<T>(string key, Func<T> valueFactory, DateTimeOffset absoluteExpiration, string group = null)
        {
            T result = Get<T>(key, group);

            if (result == null)
            {
                result = valueFactory.Invoke();

                if (result != null)
                {
                    Add(key, result, absoluteExpiration, group);
                }
            }

            return result;
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new BookstoreConverter(),
                    new BookConverter()
                    //new RoleConverter() - TODO - need to add support for membership
                }
            };
        }

        private string BuildKey(string key, string group = null)
        {
            string[] keyValues = new string[] { KeyPrefix, group, key };
            string result = string.Join("-", keyValues.Where(str => !string.IsNullOrEmpty(str)));

            return result;
        }
    }
}