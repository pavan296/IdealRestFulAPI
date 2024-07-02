using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace IdealAPI.Repository
{
    public class CacheService : ICachingInterface
    {
        private IDatabase _cacheDb;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb=redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value=_cacheDb.StringGet(key);
            if(!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public object RemoveData(string key)
        {
           var exist=_cacheDb.KeyExists(key);

            if(exist) return _cacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, TimeSpan? expirationTime)
        {
            // Ensure expirationTime is not null before subtracting from DateTime.Now
            TimeSpan expiryTime = expirationTime ?? TimeSpan.FromMinutes(30); // Default to 30 minutes if expirationTime is null

            // Serialize the value to JSON
            string jsonData = JsonConvert.SerializeObject(value);

            // Set the value in the cache with the expiration time
            return _cacheDb.StringSet(key, jsonData, expiryTime);
        }
    }
}
