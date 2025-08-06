using Microsoft.Extensions.Caching.Memory;

namespace Common.Libraries.Services.CQRS.TestApi
{
    public class GetOrderByIdCache : ICacheStrategy<GetOrderByIdQuery, OrderDto>
    {
        private readonly IMemoryCache _cache;

        public GetOrderByIdCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<OrderDto?> TryGetAsync(GetOrderByIdQuery request)
        {
            _cache.TryGetValue(GetKey(request), out OrderDto? cached);
            return Task.FromResult(cached);
        }

        public Task SetAsync(GetOrderByIdQuery request, OrderDto response)
        {
            _cache.Set(GetKey(request), response, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private string GetKey(GetOrderByIdQuery request) => $"order:{request.OrderId}";
    }

}
