using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Topics;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Topics.Sender
{
    public class SendTests : ServiceBusSpecification, IDisposable
    {
        public SendTests(ConfigurationFixture fixture) : base(fixture) { }

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
                         });
                    })
                    .AddJsonSerializers();
        }

        [ServiceBusTest]
        public async Task WhenSendAsyncCalledWithSingleNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEventNotification<IEvent>)null);

                (await act.Should().ThrowAsync<ArgumentNullException>())
                    .And.ParamName.Should().Be("context");
            }
        }
        [ServiceBusTest]
        public async Task WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEventNotification<IEvent>>)null);

                (await act.Should().ThrowAsync<ArgumentNullException>())
                    .And.ParamName.Should().Be("contexts");
            }
        }
        [ServiceBusTest]
        public async Task WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            { 
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var @event = new SameSenderEvent();
                var notification = new EventNotification<SameSenderEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null); ;

                Func<Task> act = async () => await sender.SendAsync(notification);

                await act.Should().NotThrowAsync();
            }
        }
        [ServiceBusTest]
        public async Task WhenSendAsyncCalledWithMultipleEventsThenShouldSendEvents()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var events = new[] { new SameSenderEvent(), new SameSenderEvent() };
                var notifications = events.Select(@event => new EventNotification<SameSenderEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null));
                
                Func<Task> act = async () => await sender.SendAsync(notifications);

                await act.Should().NotThrowAsync();
            }
        }

        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                if (managementClient.TopicExistsAsync(builder.EntityPath).GetAwaiter().GetResult())
                    managementClient.DeleteTopicAsync(builder.EntityPath).GetAwaiter().GetResult();
            }
        }

        private class SameSenderEvent : Event
        {
            public SameSenderEvent() : base(Guid.NewGuid().ToString(), 1)
            {
            }
        }
    }
}
