using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    public interface IEventManager
    {
        Task ProcessEvent<T>(IEvent @event, CancellationToken cancellationToken = default);
        Task ProcessEvent<T>(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    }
}
