using Microsoft.Azure.ServiceBus;

namespace SIO.Infrastructure.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create(ServiceBusSubscription subscription);
    }
}
