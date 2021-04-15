using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Queries
{
    public interface IQueryDispatcher
    {
        Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}
