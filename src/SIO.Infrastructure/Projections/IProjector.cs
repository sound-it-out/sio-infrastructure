using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SIO.Infrastructure.Projections
{
    public interface IProjector<TProcess> : IHostedService, IDisposable
    {
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
