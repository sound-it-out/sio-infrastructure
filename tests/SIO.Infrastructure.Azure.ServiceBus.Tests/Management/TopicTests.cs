using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Azure.ServiceBus.Exceptions;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Management;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Management
{
    public class TopicTests : ServiceBusSpecification, IDisposable
    {
        private readonly string _topicName = $"test-topic-{Guid.NewGuid()}";

        public TopicTests(ConfigurationFixture fixture) : base(fixture) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(o => o.AddDebug())
                    .AddSIOInfrastructure()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Configuration["Azure:ServiceBus:ConnectionString"])
                         .UseTopic(e =>
                         {
                             e.WithName(_topicName);
                             e.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();
        }

        [ServiceBusTest]
        public void WhenCreateTopicAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: null);

                act.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public void WhenCreateTopicAsyncCalledWithNonExistentTopicThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenCreateTopicAsyncCalledWithExistingTopicThenShouldThrowExchangeAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateTopicAsync(topicName: _topicName);
                };

                act.Should().Throw<TopicAlreadyExistsException>()
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public void WhenTopicExistsAsyncCalledWithTopicExchangeThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    var result = await client.TopicExistsAsync(topicName: _topicName);

                    result.Should().BeFalse();
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenTopicExistsAsyncCalledWithTopicExchangeThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var result = await client.TopicExistsAsync(topicName: _topicName);

                    result.Should().BeTrue();
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenRemoveTopicAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: null);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public void WhenRemoveTopicAsyncCalledWithExistingTopicThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                act.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: _topicName);
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenRemoveTopicAsyncCalledWithNonExistingTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: _topicName);
                };

                verify.Should().Throw<TopicNotFoundException>();
            }
        }
        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                if (managementClient.TopicExistsAsync(_topicName).GetAwaiter().GetResult())
                    managementClient.DeleteTopicAsync(_topicName).GetAwaiter().GetResult();
            }
        }
    }
}
