﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    internal sealed class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateFactory _aggregateFactory;

        public AggregateRepository(IEventStore eventStore,
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

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, @event.Timestamp, actor: Actor.From("unknown")));

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

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: causation.CorrelationId, causationId: CausationId.From(causation.Id), @event.Timestamp, actor: causation.Actor));

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

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: causation.CorrelationId, causationId: CausationId.From(causation.Payload.Id), @event.Timestamp, actor: causation.Actor));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts);

            aggregate.ClearUncommittedEvents();
        }
    }
}
