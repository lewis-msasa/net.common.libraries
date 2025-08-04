using Common.Libraries.EventSourcing;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Common.Libraries.EventStore
{
    public abstract class ReactorBase<T> : ISubscription<T> where T : class
    {
        private readonly ILogger<ReactorBase<T>> _logger;

        public ReactorBase(Reactor reactor,
            ILogger<ReactorBase<T>> logger)
        {
            _reactor = reactor;
            _logger = logger;
        }

        readonly Reactor _reactor;

        public Task Project(object @event)
        {
            var handler = _reactor(@event);

            if (handler == null) return Task.CompletedTask;
            
            _logger.LogDebug("Reacting to event {event}", @event);

            return handler();
        }
        
        public delegate Func<Task> Reactor(object @event);
    }
}