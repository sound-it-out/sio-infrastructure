using System;
using FluentAssertions;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using Xunit;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Messages.MessageFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullEventSerializerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultMessageFactory(eventSerializer: null, eventDeserializer: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("eventSerializer");
        }
    }
}
