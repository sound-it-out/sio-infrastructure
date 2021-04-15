using System;

namespace SIO.Infrastructure.Azure.ServiceBus.Exceptions
{
    public class SubscriptionAlreadyExistsException : Exception
    {
        public string SubscriptionName { get; }

        public SubscriptionAlreadyExistsException(string name)
            : base($"A subscription with name '{name}' already exists.")
        {
            SubscriptionName = name;
        }
    }
}
