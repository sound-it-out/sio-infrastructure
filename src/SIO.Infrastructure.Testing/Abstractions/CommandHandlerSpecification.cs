using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Testing.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace SIO.Infrastructure.Testing.Abstractions
{
    public abstract class CommandHandlerSpecification<TCommand> : IAsyncLifetime
        where TCommand: ICommand
    {
        private readonly IServiceScope _serviceScope;
        private readonly ICommandHandler<TCommand> _commandHandler;

        protected readonly CancellationTokenSource _cancellationTokenSource = new();

        private ExceptionMode _exceptionMode;

        protected abstract TCommand Given();
        protected abstract Type Handler();

        protected virtual Task When() => Task.CompletedTask;

        protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;
        protected Exception Exception { get; private set; }

        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;
        protected virtual void BuildServices(IServiceCollection services) {}

        public CommandHandlerSpecification(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            services.AddXunitLogging(testOutputHelper);
            services.AddScoped(typeof(ICommandHandler<TCommand>), Handler());

            BuildServices(services);
            _serviceScope = services.BuildServiceProvider().CreateScope();
            _commandHandler = ServiceProvider.GetService<ICommandHandler<TCommand>>();
        }

        public virtual Task DisposeAsync()
        {
            _serviceScope.Dispose();
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                await _commandHandler.ExecuteAsync(Given(), _cancellationTokenSource.Token);
            }
            catch (Exception e) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = e;
            }
        }
    }
}
