using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;

using FluentAssertions;
using Xunit;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Serialization;
using SIO.Infrastructure.Serialization.Json.Extensions;
using System.Text;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Messages.MessageFactory
{
    public class CreateMessageTests
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateMessageTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddSIOInfrastructure()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(t =>
                         {
                             t.WithName("test-exchange");
                             t.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }).CreateScope().ServiceProvider;
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [ServiceBusTest]
        public void WhenCreateMessageCalledWithNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();

                Action act = () => factory.CreateMessage(null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("context");
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateMessageIdFromEventId()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: CorrelationId.New(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.MessageId.Should().Be(@event.Id.ToString());
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateTypeFromEventTypeName()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: CorrelationId.New(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.Label.Should().Be(nameof(FakeEvent));
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateCorrelationIdFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: CorrelationId.New(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.CorrelationId.Should().Be(notification.CorrelationId.ToString());
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateBodyFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var serializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);
                var body = Encoding.UTF8.GetBytes(serializer.Serialize(@event));
                var result = factory.CreateMessage(notification);

                result.Body.Should().Equal(body);
                result.Size.Should().Be(body.Length);
            }
        }

        private class FakeEvent : Event
        {
            public string Message { get; } = nameof(FakeEvent);

            public FakeEvent()
                : base(Guid.NewGuid().ToString(), 1)
            {
            }
        }
    }
}
