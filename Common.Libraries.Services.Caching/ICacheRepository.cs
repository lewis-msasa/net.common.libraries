using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Caching
{
    using Common.Libraries.Services.Specification;
    using System.Text.Json;

    public class CachedRepository<T>  where T : class,ISpecEntity,HasId
    {
        private readonly IAppCache _cache;
        private readonly IRepository<T> _repository;
        private readonly ICacheSettings _settings;

        public CachedRepository(IAppCache cache, IRepository<T> repository, ICacheSettings settings)
        {
            _cache = cache;
            _repository = repository;
            _settings = settings;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var cacheKey = $"{typeof(T).Name.ToLower()}_{id}";
            var cached = await _cache.GetAsync<T>(cacheKey);

            if (cached != null)
                return cached;

            
            var entity = await _repository.FirstOrDefaultAsync(new Specification<T>().Where(c => c.Id  == id));

            if (entity != null)
            {
                await _cache.SetAsync(cacheKey,
                    JsonSerializer.Serialize(entity), TimeSpan.FromMinutes(_settings.DefaultAbsoluteExpireTime),
                    TimeSpan.FromMinutes(_settings.DefaultSlidingExpireTime));
            }

            return entity;
        }
    }

}
