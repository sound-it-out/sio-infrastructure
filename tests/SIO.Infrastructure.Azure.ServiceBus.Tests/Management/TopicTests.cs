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
        public async Task WhenCreateTopicAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: null);

                (await act.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateTopicAsyncCalledWithNonExistentTopicThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateTopicAsyncCalledWithExistingTopicThenShouldThrowExchangeAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateTopicAsync(topicName: _topicName);
                };

                (await act.Should().ThrowAsync<TopicAlreadyExistsException>())
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public async Task WhenTopicExistsAsyncCalledWithTopicExchangeThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    var result = await client.TopicExistsAsync(topicName: _topicName);

                    result.Should().BeFalse();
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenTopicExistsAsyncCalledWithTopicExchangeThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                await act.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var result = await client.TopicExistsAsync(topicName: _topicName);

                    result.Should().BeTrue();
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveTopicAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: null);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveTopicAsyncCalledWithExistingTopicThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () => await client.CreateTopicAsync(topicName: _topicName);

                await act.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: _topicName);
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveTopicAsyncCalledWithNonExistingTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveTopicAsync(topicName: _topicName);
                };

                await verify.Should().ThrowAsync<TopicNotFoundException>();
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
