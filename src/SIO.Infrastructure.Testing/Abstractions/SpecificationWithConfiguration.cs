using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Testing.Extensions;
using SIO.Infrastructure.Testing.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SIO.Infrastructure.Testing.Abstractions
{
    public abstract class SpecificationWithConfiguration<TConfigurationFixture, TResult> : IAsyncLifetime, IClassFixture<TConfigurationFixture>
        where TConfigurationFixture : BaseConfigurationFixture
    {
        private ExceptionMode _exceptionMode;
        protected abstract Task<TResult> Given();

        protected abstract Task When();

        protected Exception Exception { get; private set; }
        protected TResult Result { get; private set; }

        protected readonly TConfigurationFixture _configurationFixture;
        protected readonly IServiceProvider _serviceProvider;

        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;
        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        public SpecificationWithConfiguration(TConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper)
        {
            _configurationFixture = configurationFixture;

            var services = new ServiceCollection();
            services.AddXunitLogging(testOutputHelper);

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                Result = await Given();
            }
            catch (Exception e)
            {
                if (_exceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }

    public abstract class SpecificationWithConfiguration<TConfigurationFixture> : IAsyncLifetime, IClassFixture<TConfigurationFixture>
        where TConfigurationFixture : BaseConfigurationFixture
    {
        private ExceptionMode _exceptionMode;

        protected abstract Task Given();

        protected abstract Task When();

        protected Exception Exception { get; private set; }

        protected readonly TConfigurationFixture _configurationFixture;
        protected readonly IServiceProvider _serviceProvider;

        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;
        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        public SpecificationWithConfiguration(TConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper)
        {
            _configurationFixture = configurationFixture;

            var services = new ServiceCollection();
            services.AddXunitLogging(testOutputHelper);

            BuildServices(services);

            _serviceProvider = services.BuildServiceProvider();
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            await When();

            try
            {
                await Given();
            }
            catch (Exception e) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = e;
            }
        }
    }
}
