
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public class DataBag : IDisposable
    {
        private readonly IDataBagStore _store;

        public DataBag(IDataBagStore store)
        {
            _store = store;
        }

        public Task SetAsync<T>(string key, T value) => _store.SetAsync(key, value);
        public Task<T?> GetAsync<T>(string key) => _store.GetAsync<T>(key);
        public Task<bool> ContainsAsync(string key) => _store.ContainsAsync(key);
        public Task<bool> RemoveAsync(string key) => _store.RemoveAsync(key);
        public void Set<T>(DataBagKey<T> key, T value) => SetAsync(key.Name, value);
        public Task<T?> GetAsync<T>(DataBagKey<T> key) => GetAsync<T>(key.Name);

        public void Dispose()
        {
            _store.CloseAsync();
            
        }
    }
}
