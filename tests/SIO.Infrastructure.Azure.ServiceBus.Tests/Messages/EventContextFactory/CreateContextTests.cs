using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization;
using SIO.Infrastructure.Serialization.Json.Extensions;
using SIO.Infrastructure.Serialization.MessagePack.Extensions;
using Xunit;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Messages.EventContextFactory
{
    public class CreateContextTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateContextTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddSIOInfrastructure()
                    .AddEvents(o => o.Register<CreateTestEvent>())
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
            
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }).CreateScope().ServiceProvider;
        }
        
        [Fact]
        public void WhenReceivedEventIsNullThenShouldThrowArgumentNullException()
        {
            var factory = ServiceProvider.GetRequiredService<IEventContextFactory>();

            Action act = () => factory.CreateContext(null);

            act.Should().Throw<ArgumentNullException>()
                .And
                .ParamName
                .Should().Be("message");
        }
        [Fact]
        public void WhenReceivedEventIsNotNullThenShouldReturnEventContext()
        {
            var serializer = ServiceProvider.GetRequiredService<IEventSerializer>();
            var factory = ServiceProvider.GetRequiredService<IEventContextFactory>();
            var @event = new CreateTestEvent();
            var streamId = Guid.NewGuid().ToString();
            var causationId = CausationId.From(Guid.NewGuid().ToString());
            var correlationId = CorrelationId.New();
            var actor = "test-user";

            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = Encoding.UTF8.GetBytes(serializer.Serialize(@event)),
                Label = nameof(CreateTestEvent),
                CorrelationId = correlationId.ToString(),
                UserProperties =
                {
                    { nameof(IEventContext<IEvent>.StreamId), streamId.ToString() },
                    { nameof(IEventContext<IEvent>.CausationId), causationId.ToString() },
                    { nameof(IEventContext<IEvent>.Actor), actor },
                    { nameof(IEventContext<IEvent>.Timestamp), @event.Timestamp },
                },
            };

            var context = factory.CreateContext(message);

            context.StreamId.Should().Be(streamId);
            context.CausationId.Should().Be(causationId);
            context.CorrelationId.Should().Be(correlationId);
            context.Payload.Should().BeOfType<CreateTestEvent>();
            context.Timestamp.Should().Be(@event.Timestamp);
            context.Actor.Should().Be(Actor.From(actor));
        }

        private class CreateTestEvent : Event
        {
            public CreateTestEvent() : base(Guid.NewGuid().ToString(), 1) { }
        }
    }
}
