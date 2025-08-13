
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
        private readonly string _name;
        public DataBag(IDataBagStore store, string name)
        {
            _store = store;
            _name = name;   
        }

        public Task SetAsync<T>(string key, T value) => _store.SetAsync(key,_name, value);
        public Task<T?> GetAsync<T>(string key) => _store.GetAsync<T>(key,_name);
        public Task<bool> ContainsAsync(string key) => _store.ContainsAsync(key, _name);
        public Task<bool> RemoveAsync(string key) => _store.RemoveAsync(key, _name);
        public void Set<T>(DataBagKey<T> key, T value) => SetAsync(key.Name, value);
        public Task<T?> GetAsync<T>(DataBagKey<T> key) => GetAsync<T>(key.Name);

        public void Dispose()
        {
            _store.CloseAsync(_name);
            
        }
    }
}
