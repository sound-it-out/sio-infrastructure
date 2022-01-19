using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SIO.Infrastructure.Processing
{
    internal sealed class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<BackgroundTask> _queue;

        public BackgroundTaskQueue(IOptions<BackgroundProcessorOptions> options)
        {
            if(options is null)
                throw new ArgumentNullException(nameof(options));

            var channelOptions = new BoundedChannelOptions(options.Value.Capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _queue = Channel.CreateBounded<BackgroundTask>(channelOptions);
        }

        public async ValueTask QueueAsync(Func<IServiceScopeFactory, CancellationToken, ValueTask> task, ExecutionType executionType = ExecutionType.Await)
            => await _queue.Writer.WriteAsync(new BackgroundTask(task, executionType));

        public async ValueTask<BackgroundTask> DequeueAsync(CancellationToken cancellationToken = default)
            => await _queue.Reader.ReadAsync(cancellationToken);
    }
}
