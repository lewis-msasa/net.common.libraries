using System.Threading.Tasks;

namespace Common.Libraries.EventSourcing
{
    public interface ISubscription
    {
        Task Project(object @event);
    }
    public interface ISubscription<T> : ISubscription where T : class
    {
        Task Project(object @event);
    }
}