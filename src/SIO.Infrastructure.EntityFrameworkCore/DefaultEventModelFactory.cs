using System;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Serialization;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    internal sealed class DefaultEventModelFactory : IEventModelFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public DefaultEventModelFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
        }

        public Entities.Event Create(string streamId, IEventContext<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var type = @event.GetType();

            return new Entities.Event
            {
                StreamId = streamId,
                CorrelationId = context.CorrelationId,
                CausationId = context.CausationId,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FullName,
                Timestamp = @event.Timestamp,
                Actor = context.Actor,
                ScheduledPublication = context.ScheduledPublication
            };
        }
    }
}
