using Common.Libraries.EventSourcing;
using Common.Libraries.Services.Entities;
using Common.Libraries.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore.Projection
{
    public interface IProjectorProvider<T> where T: class, IEntity
    {
        Projector<T> GetProjector();
    }
    public delegate Func<Task> Projector<T>(
           IRepository<T> repository,
           object @event
       ) where T: class, IEntity;

    public class EFDbProjection<T> : ISubscription<T> where T : class, IEntity
    {
        readonly Projector<T> _projector;
        readonly IRepository<T> _repository;

        public EFDbProjection(IProjectorProvider<T> projector, 
            IRepository<T> repository)
        {
            _projector = projector.GetProjector();
            _repository = repository;
        }

        public async Task Project(object @event)
        {
            var handler = _projector(_repository, @event);
            if (handler == null) return;
            await handler();
            
        }
       
    }
}
