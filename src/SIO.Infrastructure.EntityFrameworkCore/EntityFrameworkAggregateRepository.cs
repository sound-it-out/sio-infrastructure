using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    internal sealed class EntityFrameworkAggregateRepository<TStoreContext> : IEntityFrameworkAggregateRepository<TStoreContext>
        where TStoreContext : DbContext, ISIOStoreDbContext
    {
        private readonly IEventStore<TStoreContext> _eventStore;
        private readonly IAggregateFactory _aggregateFactory;

        public EntityFrameworkAggregateRepository(IEventStore<TStoreContext> eventStore,
                                   IAggregateFactory aggregateFactory)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));

            _eventStore = eventStore;
            _aggregateFactory = aggregateFactory;
        }

        public async Task<TAggregate> GetAsync<TAggregate, TState>(string id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            var events = await _eventStore.GetEventsAsync(StreamId.From(id));

            if (!events.Any())
                return default;

            var aggregate = _aggregateFactory.FromHistory<TAggregate, TState>(events.Select(e => e.Payload));

            return aggregate;
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            cancellationToken.ThrowIfCancellationRequested();

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(StreamId.From(aggregate.Id));

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlationId: null,
                causationId: null,
                timestamp: @event.Timestamp,
                actor: Actor.From("unknown"),
                scheduledPublication: null));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts, cancellationToken);

            aggregate.ClearUncommittedEvents();
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            cancellationToken.ThrowIfCancellationRequested();

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(StreamId.From(aggregate.Id));

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlationId: causation.CorrelationId,
                causationId: CausationId.From(causation.Id),
                timestamp: @event.Timestamp,
                actor: causation.Actor,
                scheduledPublication: null));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts);

            aggregate.ClearUncommittedEvents();
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            cancellationToken.ThrowIfCancellationRequested();

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(StreamId.From(aggregate.Id));

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlationId: causation.CorrelationId,
                causationId: CausationId.From(causation.Payload.Id),
                timestamp: @event.Timestamp,
                actor: causation.Actor,
                scheduledPublication: causation.ScheduledPublication));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts);

            aggregate.ClearUncommittedEvents();
        }
    }
}
