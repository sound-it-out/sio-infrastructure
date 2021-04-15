using System;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Projections
{
    public interface IProjectionManager<TView>
        where TView : class, IProjection
    {
        Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default);
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
