using System;

namespace SIO.Infrastructure.Events
{
    public sealed class EventNotification<TEvent> : IEventNotification<TEvent>
        where TEvent : IEvent
    {
        public string StreamId { get; }
        public CorrelationId? CorrelationId { get; }
        public CausationId? CausationId { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public string UserId { get; }

        public EventNotification(string streamId, TEvent @event, CorrelationId? correlationId, CausationId? causationId, DateTimeOffset timestamp, string userId)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            StreamId = streamId;
            CorrelationId = correlationId;
            CausationId = causationId;
            Payload = @event;
            Timestamp = timestamp;
            UserId = userId;
        }
    }
}
