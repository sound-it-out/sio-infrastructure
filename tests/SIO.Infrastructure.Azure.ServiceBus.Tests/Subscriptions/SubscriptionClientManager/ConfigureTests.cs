using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Subscriptions;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Subscriptions.SubscriptionClientManager
{
    public class ConfigureTests : ServiceBusSpecification, IDisposable
    {
        public ConfigureTests(ConfigurationFixture fixture) : base(fixture) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(o => o.AddDebug())
                    .AddSIOInfrastructure()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Configuration["Azure:ServiceBus:ConnectionString"])
                         .UseTopic(e =>
                         {
                             e.WithName($"test-topic-{Guid.NewGuid()}");
                             e.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         })
                         .AddSubscription(s =>
                         {
                             s.UseName($"test-sub-{Guid.NewGuid()}");
                             s.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();
        }

        [ServiceBusTest]
        public async Task WhenConfigureAsyncCalledThenShouldConfigureTopic()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionClientManager>();

                Func<Task> act = async () => await manager.RegisterClientsAsync();

                await act.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var exists = await managementClient.TopicExistsAsync(topicPath: builder.EntityPath);

                    exists.Should().BeTrue();
                };

                await verify.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenConfigureAsyncCalledThenShouldConfigureSubscriptions()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<ServiceBusOptions>>();
                var managementClient = new ManagementClient(builder);
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionClientManager>();

                Func<Task> act = async () => await manager.RegisterClientsAsync();

                await act.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    var topicExists = await managementClient.TopicExistsAsync(topicPath: builder.EntityPath);

                    topicExists.Should().BeTrue();

                    foreach (var subscription in options.Value.Subscriptions)
                    {
                        var exists = await managementClient.SubscriptionExistsAsync(builder.EntityPath, subscription.Name);

                        exists.Should().BeTrue();
                    }
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

                managementClient.DeleteTopicAsync(builder.EntityPath).GetAwaiter().GetResult();
            }
        }
    }
}
