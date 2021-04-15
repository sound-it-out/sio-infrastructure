using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Events
{
    internal sealed class DefaultEventManager : IEventManager
    {
        private readonly IEventStore _eventStore;

        public DefaultEventManager(IEventStore eventStore)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));

            _eventStore = eventStore;
        }

        public async Task ProcessEvent<T>(IEvent @event, CancellationToken cancellationToken = default)
        {
            var streamId = StreamId.New();
            var context = new EventContext<IEvent>(streamId: streamId, @event: @event, correlationId: null, causationId: null, @event.Timestamp, actor: Actor.From("unknown"));
            await _eventStore.SaveAsync(streamId, new IEventContext<IEvent>[] { context }, cancellationToken);
        }

        public async Task ProcessEvent<T>(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            var streamId = StreamId.New();
            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: streamId, @event: @event, correlationId: null, causationId: null, @event.Timestamp, actor: Actor.From("unknown")));
            await _eventStore.SaveAsync(streamId, contexts, cancellationToken);
        }
    }
}
