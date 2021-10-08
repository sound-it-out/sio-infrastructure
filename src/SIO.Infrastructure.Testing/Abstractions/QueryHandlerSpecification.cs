using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Queries;
using SIO.Infrastructure.Testing.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace SIO.Infrastructure.Testing.Abstractions
{
    public abstract class QueryHandlerSpecification<TQueryHandler, TQuery, TQueryResult> : IAsyncLifetime
        where TQueryHandler : class, IQueryHandler<TQuery, TQueryResult>
        where TQuery : IQuery<TQueryResult>
    {
        private readonly IServiceScope _serviceScope;
        private readonly IQueryHandler<TQuery, TQueryResult> _queryHandler;

        protected readonly CancellationTokenSource _cancellationTokenSource = new();
        private ExceptionMode _exceptionMode;

        protected abstract TQuery Given();

        protected virtual Task When() => Task.CompletedTask;

        protected IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;
        protected Exception Exception { get; private set; }
        protected TQueryResult Result { get; private set; }        

        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;

        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        public QueryHandlerSpecification(ITestOutputHelper testOutputHelper)
        {
            var services = new ServiceCollection();
            services.AddXunitLogging(testOutputHelper);
            services.AddScoped<IQueryHandler<TQuery, TQueryResult>, TQueryHandler>();

            BuildServices(services);
            _serviceScope = services.BuildServiceProvider().CreateScope();
            _queryHandler = ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
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
                Result = await _queryHandler.RetrieveAsync(Given(), _cancellationTokenSource.Token);
            }
            catch (Exception e) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = e;
            }
        }
    }
}
