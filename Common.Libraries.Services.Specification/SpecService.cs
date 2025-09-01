using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Specification
{
    public interface IService<T, TD>
        where T : class, ISpecEntity
        where TD : class,ISpecDto
    {
        Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);
        Task<ICollection<TD>> GetAsync(CancellationToken cancellationToken = default);
        Task<TD> GetOneAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);
    }

    public class Service<T, TD> : IService<T, TD> where T : class, ISpecEntity
         where TD : class,ISpecDto
    {
        private readonly IRepository<T> _repository;
        private readonly IEntityMapper<T, TD> _mapper;

        public Service(IRepository<T> repository, IEntityMapper<T, TD> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<TD>> GetAsync(CancellationToken cancellationToken = default)
        {
            var result = await _repository.FindAsync(cancellationToken: cancellationToken);
            return _mapper.Map(result.ToList());
        }

        public async Task<TD> GetOneAsync(
            ISpecification<T>? spec = null,
            CancellationToken cancellationToken = default)
        {
            var ent = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
            return _mapper.Map(ent);
        }
        public async Task<int> CountAsync(
            ISpecification<T>? spec = null,
            CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(spec, cancellationToken);
        }
    }

}
