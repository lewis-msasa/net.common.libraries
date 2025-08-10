using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public sealed class DataBagKey<T>
    {
        public string Name { get; set; }
        public DataBagKey(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public override string ToString() => Name;
    }
}
