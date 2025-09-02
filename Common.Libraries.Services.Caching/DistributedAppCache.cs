using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    using Microsoft.Extensions.Caching.Distributed;
    using System.Text.Json;

    public class DistributedAppCache : IAppCache
    {
        private readonly IDistributedCache _cache;

        public DistributedAppCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cached = await _cache.GetStringAsync(key);
            return cached == null ? default : JsonSerializer.Deserialize<T>(cached);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;
            if (slidingExpireTime.HasValue)
                options.SlidingExpiration = slidingExpireTime;

            var serialized = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serialized, options);
        }

        public Task RemoveAsync(string key) => _cache.RemoveAsync(key);

        // Read-Through
        public async Task<T?> GetOrLoadAsync<T>(string key, Func<Task<T?>> fetchFunc, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null)
                return cached;

            var data = await fetchFunc();
            if (data != null)
                await SetAsync(key, data, absoluteExpireTime, slidingExpireTime);

            return data;
        }

        // Write-Through
        public async Task SetAndPersistAsync<T>(string key, T value, Func<T, Task> persistFunc, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            await persistFunc(value); // persist to DB
            await SetAsync(key, value, absoluteExpireTime, slidingExpireTime);
        }
    }


}
