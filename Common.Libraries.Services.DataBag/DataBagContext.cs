using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public static class DataBagContext
    {
        private static readonly AsyncLocal<DataBag?> _current = new();
        private static Func<IDataBagStore> _storeFactory = () => new DictionaryDataStore();

        public static void ConfigureStore(IServiceProvider sp)
        {
            _storeFactory = () => sp.GetRequiredService<IDataBagStore>();
        }
        public static void ConfigureStore(Func<IDataBagStore> factory)
        {
            _storeFactory = factory;
        }
        public static DataBag Current
        {
            get
            {
                if(_current.Value == null) _current.Value = new DataBag(_storeFactory());
                return _current.Value;
            }
            set => _current.Value = value;
        }
        public static void Clear() => _current.Value = null;    
    }
}
