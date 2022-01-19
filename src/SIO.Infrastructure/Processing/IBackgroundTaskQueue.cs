using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.Processing
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueAsync(Func<IServiceScopeFactory, CancellationToken, ValueTask> task, ExecutionType executionType = ExecutionType.Await);
        internal ValueTask<BackgroundTask> DequeueAsync(CancellationToken cancellationToken = default);
    }
}
