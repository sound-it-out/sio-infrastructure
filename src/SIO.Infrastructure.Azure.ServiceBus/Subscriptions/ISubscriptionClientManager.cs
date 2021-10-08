using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SIO.Infrastructure.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientManager
    {
        Task<IReadOnlyList<ISubscriptionClient>> RegisterClientsAsync();
        Task ConfigureSubscriptionsAsync();
    }
}
