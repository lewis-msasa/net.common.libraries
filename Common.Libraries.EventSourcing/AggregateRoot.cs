using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Libraries.EventSourcing
{
    public interface ISnapshot
    {
        Guid AggregateId { get; }
        int Version { get; }
        public DateTime Timestamp { get; set; }
    }
    public abstract class AggregateRoot<TSnapshot> : IInternalEventHandler where TSnapshot : ISnapshot
    {
        readonly List<object> _changes = new List<object>();

        public Guid Id { get; protected set; }

        public int Version { get;  set; } = -1;

        void IInternalEventHandler.Handle(object @event) => When(@event);

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            EnsureValidState();
            _changes.Add(@event);
        }

        public IEnumerable<object> GetChanges() => _changes.AsEnumerable();

        public void Load(IEnumerable<object> history)
        {
            foreach (var e in history)
            {
                When(e);
                Version++;
            }
        }

        public void LoadFromHistory(TSnapshot? snapshot, IEnumerable<object> events)
        {
            if (snapshot is not null)
                LoadSnapshot(snapshot);

            Load(events);
        }

        protected abstract void LoadSnapshot(TSnapshot snapshot);

        public abstract TSnapshot CreateSnapShot();

        public void ClearChanges() => _changes.Clear();

        protected abstract void EnsureValidState();

        protected void ApplyToEntity(IInternalEventHandler entity, object @event) => entity?.Handle(@event);
    }
}