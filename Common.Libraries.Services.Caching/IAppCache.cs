using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    public interface IAppCache
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
        Task RemoveAsync(string key);

        Task<T?> GetOrLoadAsync<T>(string key, Func<Task<T?>> fetchFunc, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);

       
        Task SetAndPersistAsync<T>(string key, T value, Func<T, Task> persistFunc, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null);
    }

}
