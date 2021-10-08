using System;
using FluentAssertions;
using Moq;
using SIO.Infrastructure.Azure.ServiceBus.Management;
using SIO.Infrastructure.Azure.ServiceBus.Topics;
using Xunit;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests.Topics.TopicClientFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullManagementClientThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultTopicClientFactory(managementClient: null, connectionStringBuilder: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("managementClient");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionStringBuilderThenShouldThrowArgumentNullException()
        {
            var client = Mock.Of<IServiceBusManagementClient>();

            Action act = () => new DefaultTopicClientFactory(managementClient: client, connectionStringBuilder: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionStringBuilder");
        }
    }
}
