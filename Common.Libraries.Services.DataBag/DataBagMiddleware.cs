using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public class DataBagMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDataBagStore _dataBagStore;
        public DataBagMiddleware(RequestDelegate next, IDataBagStore dataBagStore)
        {
            _next = next;
            _dataBagStore = dataBagStore;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var bag = new DataBag(_dataBagStore);
            context.Items[DataBagConstants.BagKey] = bag;
            DataBagContext.Current = bag;
            try
            {
                await _next(context);
            }
            finally
            {
                DataBagContext.Clear();
            }
        }
    }
}
