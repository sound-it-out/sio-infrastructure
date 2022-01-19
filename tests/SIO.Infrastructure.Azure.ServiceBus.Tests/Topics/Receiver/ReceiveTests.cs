using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;
using SIO.Infrastructure.Serialization.MessagePack.Extensions;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Topics.Receiver
{
    public class ReceiveTests : ServiceBusSpecification, IDisposable
    {
        public ReceiveTests(ConfigurationFixture fixture) : base(fixture) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(o => o.AddDebug())
                    .AddSIOInfrastructure()
                    .AddEvents(o => o.Register<SampleReceiverEvent>())
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
                             s.UseName($"receiver-{Guid.NewGuid()}");
                             s.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                             s.ForEvent<SampleReceiverEvent>();
                         });
                    })
                    .AddJsonSerializers();
            services.AddTransient<IEventHandler<SampleReceiverEvent>, SampleReceiverEventHandler>();
        }

        [ServiceBusTest]
        public async Task WhenStartCalledThenShouldConsumePublishedEventWithSingleSubscription()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var @event = new SampleReceiverEvent();
                var notification = new EventNotification<SampleReceiverEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null);
                var sender = scope.ServiceProvider.GetRequiredService<IEventBusPublisher>();
                var receiver = scope.ServiceProvider.GetRequiredService<IEventBusConsumer>();
                var sentTime = DateTimeOffset.MinValue;

                Func<Task> act = async () => await receiver.StartAsync(CancellationToken.None);

                await act.Should().NotThrowAsync();

                Func<Task> verify = async () =>
                {
                    // Let the consumer actually startup, needs to open a connection which may take a short amount of time.
                    await Task.Delay(250);

                    await sender.PublishAsync(notification);
                    sentTime = DateTimeOffset.UtcNow;

                    // Delay to ensure that we pick up the message.
                    await Task.Delay(500);
                };

                await verify.Should().NotThrowAsync();

                SampleReceiverEventHandler.Received.Should().Be(1);
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

        private class SampleReceiverEvent : Event
        {
            public SampleReceiverEvent() : base(Guid.NewGuid().ToString(), 1)
            {
            }
        }
        private class SampleReceiverEventHandler : IEventHandler<SampleReceiverEvent>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(IEventContext<SampleReceiverEvent> context, CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
    }
}
