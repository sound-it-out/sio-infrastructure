using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    public interface IEventStore<TStoreContext>
        where TStoreContext : IStoreContext
    {
        Task<Page> GetEventsAsync(long offset, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, DateTimeOffset timeStamp, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, long offset, CancellationToken cancellationToken = default);
        Task<IEventContext<IEvent>> GetEventAsync(Subject subject, CancellationToken cancellationToken = default);
        Task SaveAsync(StreamId streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default);
        Task<long> CountAsync(StreamId streamId, CancellationToken cancellationToken = default);
    }
}
