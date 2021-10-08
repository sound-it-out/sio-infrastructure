using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken = default);
    }
}
