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
    public class SubscriptionTests : ServiceBusSpecification, IDisposable
    {
        private readonly string _topicName = $"test-topic-{Guid.NewGuid()}";

        public SubscriptionTests(ConfigurationFixture fixture) : base(fixture) { }

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
        public async Task WhenCreateSubscriptionAsyncCalledWithNullTopicNameThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateSubscriptionAsync(subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateSubscriptionAsyncCalledWithNullSubscriptionNameThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateSubscriptionAsync(subscriptionName: null, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateSubscriptionAsyncCalledWithNonExistentSubscriptionThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateSubscriptionAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var topicName = $"test-topic-{Guid.NewGuid()}";
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: topicName);

                (await act.Should().ThrowAsync<TopicNotFoundException>())
                    .And.TopicName.Should().Be(topicName);
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateSubscriptionAsyncCalledWithExistingSubscriptionThenShouldThrowSubscriptionAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                (await act.Should().ThrowAsync<SubscriptionAlreadyExistsException>())
                    .And.SubscriptionName.Should().Be(subscriptionName);
            }
        }
        [ServiceBusTest]
        public async Task WhenSubscriptionExistsAsyncCalledWithExistingSubscriptionThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeTrue();
                };

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenSubscriptionExistsAsyncCalledWithNonExistingTopicThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    //await client.CreateTopicAsync(topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeFalse();
                };

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenSubscriptionExistsAsyncCalledWithNonExistingSubscriptionThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeFalse();
                };

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveSubscriptionAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                (await act.Should().ThrowAsync<TopicNotFoundException>())
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveSubscriptionAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                (await act.Should().ThrowAsync<SubscriptionNotFoundException>())
                    .And.SubscriptionName.Should().Be(subscriptionName);
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveSubscriptionAsyncCalledWithExistingSubscriptionThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                await act.Should().NotThrowAsync();
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
