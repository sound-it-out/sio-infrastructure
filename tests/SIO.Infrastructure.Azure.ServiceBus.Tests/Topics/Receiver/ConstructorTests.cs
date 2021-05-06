using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using SIO.Infrastructure.Azure.ServiceBus.Topics;
using Xunit;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Topics.Receiver
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultTopicMessageReceiver(logger: null, eventContextFactory: null, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullEventContextFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultTopicMessageReceiver>>();

            Action act = () => new DefaultTopicMessageReceiver(logger: logger, eventContextFactory: null, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("eventContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullTopicClientFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultTopicMessageReceiver>>();
            var eventContextFactory = Mock.Of<IEventContextFactory>();

            Action act = () => new DefaultTopicMessageReceiver(logger: logger, eventContextFactory: eventContextFactory, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceScopeFactory");
        }
    }
}
