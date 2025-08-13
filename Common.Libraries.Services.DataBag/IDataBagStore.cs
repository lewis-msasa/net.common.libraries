using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public interface IDataBagStore
    {
        Task SetAsync<T>(string key, string storeName, T value, CancellationToken cancellationToken = default!);
        Task<T?> GetAsync<T>(string key, string storeName, CancellationToken cancellationToken = default!);
        Task<bool> RemoveAsync(string key, string storeName, CancellationToken cancellationToken = default!);
        Task<bool> ContainsAsync(string key, string storeName, CancellationToken cancellationToken = default!);
        Task CloseAsync(string storeName, CancellationToken cancellationToken = default!);
    }
    public class DictionaryDataStore : IDataBagStore
    {
        private readonly ConcurrentDictionary<string, object> _store = new();
        public Task<bool> ContainsAsync(string key, string storeName, CancellationToken cancellationToken = default!)
        {
           
            return Task.FromResult(_store.ContainsKey(key));
        }

        public Task<bool> RemoveAsync(string key, string storeName, CancellationToken cancellationToken = default!)
        {
            return Task.FromResult(_store.TryRemove(key, out _));
        }

        public Task<T?> GetAsync<T>(string key, string storeName, CancellationToken cancellationToken = default!)
        {
           if (_store.TryGetValue(key, out var val) && val is T typed)
            {
                return Task.FromResult<T?>(typed);
            }
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, string storeName, T value, CancellationToken cancellationToken = default!)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task CloseAsync(string storeName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
