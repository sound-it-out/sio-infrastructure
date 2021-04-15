using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Projections
{
    public abstract class ProjectionManager<TView> : IProjectionManager<TView>
        where TView : class, IProjection
    {

        private readonly Dictionary<Type, Func<IEvent, CancellationToken, Task>> _handlers;
        protected readonly ILogger<ProjectionManager<TView>> _logger;

        public ProjectionManager(ILogger<ProjectionManager<TView>> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _handlers = new Dictionary<Type, Func<IEvent, CancellationToken, Task>>();
        }

        protected void Handle<TEvent>(Func<TEvent, CancellationToken, Task> func) => _handlers.Add(typeof(TEvent), (@event, cancellationToken) => func((TEvent)@event, cancellationToken));

        public async Task HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionManager<TView>)}.{nameof(HandleAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var type = @event.GetType();

            if (!_handlers.TryGetValue(type, out var handler))
                _logger.LogInformation($"Could not find handler for event type of '{type.Name}'");

            await handler(@event, cancellationToken);
        }

        public abstract Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
