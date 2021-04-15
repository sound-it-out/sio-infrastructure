using System.Collections.Generic;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Domain
{
    public interface IAggregateFactory
    {
        TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent> events)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
    }
}
