using Microsoft.Azure.ServiceBus;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Azure.ServiceBus.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventNotification<IEvent> context);
    }
}
