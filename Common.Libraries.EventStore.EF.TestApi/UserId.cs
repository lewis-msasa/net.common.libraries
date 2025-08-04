using Common.Libraries.EventSourcing;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class UserId : AggregateId<User>
    {
        UserId(Guid value) : base(value) { }

        public static UserId FromGuid(Guid value)
            => new UserId(value);
    }
}
