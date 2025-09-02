using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    public class CacheSettings : ICacheSettings
    {
        public int DefaultAbsoluteExpireTime { get; set; }
        public int DefaultSlidingExpireTime { get; set; }
    }
}
