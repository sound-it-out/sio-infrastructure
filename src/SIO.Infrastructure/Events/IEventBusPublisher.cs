using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
