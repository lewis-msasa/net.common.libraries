using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    using Microsoft.Extensions.Caching.Memory;

    public class InMemoryAppCache : IAppCache
    {
        private readonly IMemoryCache _cache;

        public InMemoryAppCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
                return Task.FromResult((T?)value);
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;
            if (slidingExpireTime.HasValue)
                options.SlidingExpiration = slidingExpireTime;

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        // Read-Through
        public async Task<T?> GetOrLoadAsync<T>(string key, Func<Task<T?>> fetchFunc, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            if (_cache.TryGetValue(key, out var value))
                return (T?)value;

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
