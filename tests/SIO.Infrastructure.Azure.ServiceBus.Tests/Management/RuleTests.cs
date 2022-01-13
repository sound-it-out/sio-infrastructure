using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Azure.ServiceBus.Exceptions;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Management;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Management
{
    public class RuleTests : ServiceBusSpecification, IDisposable
    {
        private readonly string _topicName = $"test-topic-{Guid.NewGuid()}";
        private readonly string _subscriptionName = $"test-sub-{Guid.NewGuid()}";
        private readonly string _ruleName = $"test-rule-{Guid.NewGuid()}";

        public RuleTests(ConfigurationFixture fixture) : base(fixture) { }

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
        public async Task WhenCreateRuleAsyncCalledWithNullRuleThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: null, subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("ruleName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: null, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<TopicNotFoundException>())
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: _subscriptionName, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<SubscriptionNotFoundException>())
                    .And.SubscriptionName.Should().Be(_subscriptionName);
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithNonExistentRuleThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeTrue();
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenCreateRuleAsyncCalledWithExistingRuleThenShouldThrowRuleAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<RuleAlreadyExistsException>())
                    .And.RuleName.Should().Be(_ruleName);
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNullRuleThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: null, subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("ruleName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: null, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () =>
                {
                    await client.RemoveRuleAsync(ruleName:_ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                (await act.Should().ThrowAsync<TopicNotFoundException>())
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> act = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                (await act.Should().ThrowAsync<SubscriptionNotFoundException>())
                    .And.SubscriptionName.Should().Be(_subscriptionName);
            }
        }
        [ServiceBusTest]
        public async Task WhenRemoveRuleAsyncCalledWithNonExistentRuleThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeTrue();

                    await client.RemoveRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeFalse();
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenRetrieveRulesAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RetrieveRulesAsync(subscriptionName: null, topicName: _topicName);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRetrieveRulesAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RetrieveRulesAsync(subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                (await verify.Should().ThrowAsync<ArgumentException>())
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public async Task WhenRetrieveRulesAsyncCalledWithSubscriptionWhichHasSingleRuleThenShouldReturnExpectedRule()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName, _subscriptionName, _topicName);
                    await client.CreateRuleAsync(nameof(SampleEvent), _subscriptionName, _topicName);
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var rules = await client.RetrieveRulesAsync(subscriptionName: _subscriptionName, topicName: _topicName);

                    rules.Should().HaveCount(1);
                    rules.Should().OnlyContain(r => r.Rule == nameof(SampleEvent));
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenRetrieveRulesAsyncCalledWithSubscriptionWhichHasMultipleRulesThenShouldReturnExpectedRules()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var ruleCount = 50;
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName, _subscriptionName, _topicName);

                    for (var i = 0; i < ruleCount; i++)
                    {
                        await client.CreateRuleAsync($"{nameof(SampleEvent)}_{i}", _subscriptionName, _topicName);
                    }
                };

                await setup.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                { 
                    var rules = await client.RetrieveRulesAsync(subscriptionName: _subscriptionName, topicName: _topicName);

                    rules.Should().HaveCount(ruleCount);
                    rules.Should().OnlyContain(r => r.Rule.StartsWith($"{nameof(SampleEvent)}_"));
                };

                await verify.Should().NotThrowAsync();
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

        private class SampleEvent : Event
        {
            public SampleEvent() : base(Guid.NewGuid().ToString(), 1)
            {
            }
        }
        private class SampleEventHandler : IEventHandler<SampleEvent>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(IEventContext<SampleEvent> context, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
    }
}
