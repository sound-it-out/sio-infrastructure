using System;

namespace SIO.Infrastructure.Events
{
    public interface IEventContext<out TEvent> where TEvent : IEvent
    {
        string StreamId { get; }
        CorrelationId? CorrelationId { get; }
        CausationId? CausationId { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        Actor Actor { get; }
        DateTimeOffset? ScheduledPublication { get; }
    }
}
