using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Processing
{
    internal sealed class BackgroundTaskProcessor : BackgroundService
    {
        private readonly ILogger<BackgroundTaskProcessor> _logger;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BackgroundTaskProcessor(ILogger<BackgroundTaskProcessor> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (logger is null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory is null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _backgroundTaskQueue = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IBackgroundTaskQueue>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var backgroundTask = await _backgroundTaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    if(backgroundTask.ExecutionType == ExecutionType.Await)
                        await backgroundTask.Task(_serviceScopeFactory, stoppingToken);

                    if(backgroundTask.ExecutionType == ExecutionType.FireAndForget)
                        _ = backgroundTask.Task(_serviceScopeFactory, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred executing {backgroundTask}.");
                }
            }
        }
    }
}
