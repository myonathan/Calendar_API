using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Koobits.Service
{
    public static class RedisCacheClientExtensions
    {
        public static async Task SetValueInCache<T>(this IRedisDatabase cache, string key, T value, TimeSpan TTL = default)
        {
            if (TTL == default)
                TTL = TimeSpan.FromHours(3);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = TTL;
            var zeroTimeSpan = new TimeSpan(0, 0, 1);
            if (TTL == zeroTimeSpan)
            {
                await cache.AddAsync<T>(key, value);
            }
            else
            {
                await cache.AddAsync<T>(key, value, TTL);
            }
        }
        // get
        public static async Task<T> GetValueFromCache<T>(this IRedisDatabase cache, string key)
        {
            var value = await cache.GetAsync<T>(key);
            return value;
        }
        public static async Task<T> GetValueFromCache<T>(this IRedisDatabase cache, string key, Func<T> callbackifnotexists, TimeSpan ts)
        {
            var value = await cache.GetAsync<T>(key);
            if (value == null)
            {
                T data = callbackifnotexists.Invoke();
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                options.AbsoluteExpirationRelativeToNow = ts;

                await SetValueInCache<T>(cache, key, data, ts);
                return await Task.FromResult(data);
            }
            return value;
        }

        public static async Task<T> GetValueFromCache<T>(this IRedisDatabase cache, string key, Func<Task<T>> callbackifnotexists, TimeSpan ts)
        {
            var value = await cache.GetAsync<T>(key);
            if (value == null)
            {
                T data = await callbackifnotexists.Invoke();
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                options.AbsoluteExpirationRelativeToNow = ts;

                await SetValueInCache<T>(cache, key, data, ts);
                return await Task.FromResult(data);
            }
            return value;
        }
        // verify if an object exists
        public static async Task<bool> ExistObjectAsync<T>(this IRedisDatabase cache, string key)
        {
            var value = await cache.ExistsAsync(key);
            return value == null ? false : true;
        }

        public static async Task HashSetValuesAsync<T>(this IRedisDatabase cache, string hashkey, T value, TimeSpan TTL = default)
        {
            if (TTL == default)
                TTL = TimeSpan.FromHours(3);

            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                string propertyName = p.Name;
                var propertyValue = p.GetValue(value);
                values.Add(propertyName, propertyValue);
            }
            await cache.HashSetAsync(hashkey, values);

            var zeroTimeSpan = new TimeSpan(0, 0, 1);
            if (TTL != zeroTimeSpan)
            {
                await cache.UpdateExpiryAsync(hashkey, TTL);
            }

        }

        public static async Task<bool> IsAllKeyExistAsync<T>(this IRedisDatabase cache, string hashkey)
        {
            var hashKeys = await cache.HashKeysAsync(hashkey);
            List<bool> result = new List<bool>();
            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                string propertyName = p.Name;

                var isExist = hashKeys.Contains(propertyName);
                result.Add(isExist);
            }
            var isAllExist = result.All(x => x);
            return isAllExist;

        }
    }
}
