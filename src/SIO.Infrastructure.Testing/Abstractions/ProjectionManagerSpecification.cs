using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Projections;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Testing.Extensions;
using Xunit.Abstractions;
using System.Threading;

namespace SIO.Infrastructure.Testing.Abstractions
{
    public abstract class ProjectionManagerSpecification<TProjection> : IAsyncLifetime
        where TProjection : class, IProjection
    {
        private readonly IServiceScope _serviceScope;        
        private readonly IProjectionManager<TProjection> _projectionManager;

        protected readonly CancellationTokenSource _cancellationTokenSource = new();

        private ExceptionMode _exceptionMode;
        protected TProjection Projection { get; private set; }
        protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;
        protected Exception Exception { get; private set; }

        protected abstract Type ProjectionManager();

        protected abstract IEnumerable<IEvent> Given();
        protected virtual void When() { }
        protected void RecordExceptions()
        {
            _exceptionMode = ExceptionMode.Record;
        }

        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        protected ProjectionManagerSpecification(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            services.AddSIOInfrastructure()
                .AddEntityFrameworkCore();
            services.AddXunitLogging(testOutputHelper);
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddDbContext<SIOProjectionDbContext>((sp, options) =>
             {
                 options.UseInMemoryDatabase($"Projection_{Subject.New()}");
             });
            services.AddScoped(typeof(IProjectionManager<TProjection>), ProjectionManager());

            BuildServices(services);

            _serviceScope = services.BuildServiceProvider().CreateScope();
            _projectionManager = _serviceScope.ServiceProvider.GetRequiredService<IProjectionManager<TProjection>>();
        }

        public async Task InitializeAsync()
        {
            try
            {
                var events = Given();

                When();

                foreach (var @event in events)
                    await _projectionManager.HandleAsync(@event, _cancellationTokenSource.Token);

                var dbContextFacotry = _serviceScope.ServiceProvider.GetRequiredService<ISIOProjectionDbContextFactory>();

                using (var context = dbContextFacotry.Create())
                    Projection = await context.Set<TProjection>().FirstOrDefaultAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = ex;
            }
        }

        public Task DisposeAsync()
        {
            _serviceScope.Dispose();
            return Task.CompletedTask;
        }
    }
}
