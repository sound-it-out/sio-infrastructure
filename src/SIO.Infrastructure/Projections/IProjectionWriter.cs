using System;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Projections
{
    public interface IProjectionWriter<TView>
        where TView : class, IProjection
    {
        Task<TView> AddAsync(string subject, Func<TView> add, CancellationToken cancellationToken = default);
        Task<TView> UpdateAsync(string subject, Func<TView, TView> update, CancellationToken cancellationToken = default);
        Task<TView> UpdateAsync(string subject, Action<TView> update, CancellationToken cancellationToken = default);
        Task RemoveAsync(string subject, CancellationToken cancellationToken = default);
        Task ResetAsync(CancellationToken cancellationToken = default);
        Task<TView> RetrieveAsync(string subject, CancellationToken cancellationToken = default);
    }
}
