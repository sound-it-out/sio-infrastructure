using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreEventStore : IEventStore
    {
        private static readonly int DefaultReloadInterval = 2000;
        private static readonly int DefaultPageSize = 500;

        private readonly ISIOStoreDbContextFactory _dbContextFactory;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IEventModelFactory _eventModelFactory;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly ILogger<EntityFrameworkCoreEventStore> _logger;

        public EntityFrameworkCoreEventStore(ISIOStoreDbContextFactory dbContextFactory,
                                             IEventContextFactory eventContextFactory,
                                             IEventModelFactory eventModelFactory,
                                             IEventTypeCache eventTypeCache,
                                             ILogger<EntityFrameworkCoreEventStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (eventContextFactory == null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (eventModelFactory == null)
                throw new ArgumentNullException(nameof(eventModelFactory));
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _eventContextFactory = eventContextFactory;
            _eventModelFactory = eventModelFactory;
            _eventTypeCache = eventTypeCache;
            _logger = logger;
        }

        public async Task<long> CountAsync(StreamId streamId, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var count = await context.Events.LongCountAsync(@event => @event.StreamId == streamId, cancellationToken);

                return count;
            }
        }

        public async Task<Page> GetEventsAsync(long offset, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsInternalAsync(offset, cancellationToken).ConfigureAwait(false);

            if (events.Count > 0 && events[0].SequenceNo != offset + 1)
            {
                _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", offset + 1, events[0].SequenceNo, DefaultReloadInterval);
                events = await GetAllEventsAfterDelayInternalAsync(offset, cancellationToken).ConfigureAwait(false);
            }

            for (var i = 0; i < events.Count - 1; i++)
            {
                if (events[i].SequenceNo + 1 != events[i + 1].SequenceNo)
                {
                    _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", events[i].SequenceNo + 1, events[i + 1].SequenceNo, DefaultReloadInterval);
                    events = await GetAllEventsAfterDelayInternalAsync(offset, cancellationToken).ConfigureAwait(false);
                    break;
                }
            }

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    continue;

                var result = _eventContextFactory.CreateContext(@event);

                results.Add(result);
            }

            return new Page(offset + events.Count, offset, results);
        }
        public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, 0, cancellationToken).ConfigureAwait(false);

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    continue;

                var result = _eventContextFactory.CreateContext(@event);

                results.Add(result);
            }

            return results;

        }
        public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, long offset, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, offset, cancellationToken).ConfigureAwait(false);

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out _))
                    continue;

                var result = _eventContextFactory.CreateContext(@event);

                results.Add(result);
            }

            return results;
        }

        public async Task<IEventContext<IEvent>> GetEventAsync(Subject subject, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var @event = await context.Events.AsNoTracking()
                                                 .FirstOrDefaultAsync(e => e.Id == subject, cancellationToken);
                if(@event == null)
                    throw new InvalidOperationException($"Unable to find event: {subject}");

                if (!_eventTypeCache.TryGet(@event.Type, out _))
                    throw new InvalidOperationException($"Unable to find event type: {@event.Type}");

                return _eventContextFactory.CreateContext(@event);
            }
        }

        public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, DateTimeOffset timeStamp, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, timeStamp, cancellationToken).ConfigureAwait(false);

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                var result = _eventContextFactory.CreateContext(@event);

                results.Add(result);
            }

            return results;
        }

        public async Task SaveAsync(StreamId streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(streamId, @event));

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsInternalAsync(long offset, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events.OrderBy(e => e.SequenceNo)
                                                 .Where(e => e.SequenceNo >= offset)
                                                 .Take(DefaultPageSize)
                                                 .AsNoTracking()
                                                 .ToListAsync(cancellationToken);

                return events;
            }
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, DateTimeOffset timeStamp, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events.OrderBy(e => e.SequenceNo)
                                                 .Where(e => e.Timestamp >= timeStamp)
                                                 .Where(e => e.StreamId == streamId)
                                                 .Take(DefaultPageSize)
                                                 .AsNoTracking()
                                                 .ToListAsync(cancellationToken);

                return events;
            }
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, long offset, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events.OrderBy(e => e.SequenceNo)
                                                 .Where(e => e.SequenceNo >= offset)
                                                 .Where(e => e.StreamId == streamId)
                                                 .Take(DefaultPageSize)
                                                 .AsNoTracking()
                                                 .ToListAsync(cancellationToken);

                return events;
            }
        }
        private async Task<List<Entities.Event>> GetAllEventsAfterDelayInternalAsync(long offset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Reloading events after {DefaultReloadInterval}ms.", DefaultReloadInterval);

            await Task.Delay(DefaultReloadInterval).ConfigureAwait(false);
            return await GetAllEventsForwardsInternalAsync(offset, cancellationToken).ConfigureAwait(false);
        }
    }
}
