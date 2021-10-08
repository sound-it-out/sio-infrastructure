using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Domain
{
    public interface IAggregateRepository
    {
        Task<TAggregate> GetAsync<TAggregate, TState>(string id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new();
    }
}
