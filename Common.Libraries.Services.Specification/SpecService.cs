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
        Task<ICollection<TD>> GetAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);
        Task<TD?> GetOneAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default);

        Task<TD?> CreateAsync(T entity, CancellationToken cancellationToken = default);

        Task<TD?> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }

    public class Service<T, TD> : IService<T, TD> where T : class, ISpecEntity
         where TD : class,ISpecDto
    {
        private readonly IRepository<T> _repository;
        private readonly IEntityMapper<T, TD> _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IRepository<T> repository, IEntityMapper<T, TD> mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            if(_mapper == null) throw new ArgumentNullException($"{nameof(mapper)} not found");
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<TD?> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var ent = await _repository.AddAsync(entity, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if(saved == 0) return null; 
            return _mapper.Map(ent);
        }
        public async Task<ICollection<TD>> GetAsync(ISpecification<T>? spec = null, CancellationToken cancellationToken = default)
        {
            var result = await _repository.FindAsync(spec: spec,cancellationToken: cancellationToken);
            return _mapper.Map(result.ToList());
        }

        public async Task<TD?> GetOneAsync(
            ISpecification<T>? spec = null,
            CancellationToken cancellationToken = default)
        {
            var ent = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
            if(ent == null) return null;    
            return _mapper.Map(ent);
        }
        public async Task<int> CountAsync(
            ISpecification<T>? spec = null,
            CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(spec, cancellationToken);
        }

        public async Task<TD?> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateAsync(entity, cancellationToken);
            var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (saved == 0) return null;
            return _mapper.Map(entity);
        }

        public async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
           await _repository.DeleteAsync(entity, cancellationToken);
           var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);
           return saved > 0;
        }
    }

}
