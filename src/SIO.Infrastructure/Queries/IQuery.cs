using System;

namespace SIO.Infrastructure.Queries
{
    public interface IQuery<out TQueryResult>
    {
        QueryId Id { get; }
        DateTimeOffset Timestamp { get; }
        CorrelationId? CorrelationId { get; }
        Actor Actor { get; }
    }
}
