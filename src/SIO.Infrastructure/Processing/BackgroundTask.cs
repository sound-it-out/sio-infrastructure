using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.Processing
{
    internal sealed record BackgroundTask(Func<IServiceScopeFactory, CancellationToken, ValueTask> Task, ExecutionType ExecutionType = ExecutionType.Await);
}
