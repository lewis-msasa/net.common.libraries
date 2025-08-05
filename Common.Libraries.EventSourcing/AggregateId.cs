using System;

namespace Common.Libraries.EventSourcing
{
    public abstract class AggregateId<T,TSnapshot> : Value<AggregateId<T,TSnapshot>>
        where T : AggregateRoot<TSnapshot>, new () where TSnapshot : ISnapshot
    {
        protected AggregateId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(
                    nameof(value), 
                    "The Id cannot be empty");
            
            Value = value;
        }

        public Guid Value { get; }
        
        public static implicit operator Guid(AggregateId<T, TSnapshot> self) => self.Value;

        public override string ToString() => Value.ToString();
    }
}