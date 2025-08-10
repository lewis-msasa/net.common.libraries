
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.DataBag
{
    public static class HttpContextExtensions
    {
        private static DataBag GetOrCreateDataBag(HttpContext ctx)
        {
            if(ctx.Items.TryGetValue(DataBagConstants.BagKey, out var bagObj) && bagObj is DataBag bagFromItems)
            {
                return bagFromItems;
            }
            var store = ctx.RequestServices.GetRequiredService<IDataBagStore>();
            var bag = new DataBag(store);
            ctx.Items[DataBagConstants.BagKey] = bag;
            return bag;
        }
        public static void SetValue<T>(this HttpContext ctx, DataBagKey<T> key, T value)
        {
            var bag = GetOrCreateDataBag(ctx);
            bag.Set(key,value);
        }
        public static async Task<T?> GetValue<T>(this HttpContext ctx, DataBagKey<T> key)
        {
            var bag = GetOrCreateDataBag(ctx);
            return await bag.GetAsync(key);
        }
       
    }
}
