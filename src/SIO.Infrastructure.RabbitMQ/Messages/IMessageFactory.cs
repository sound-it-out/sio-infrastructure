using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventNotification<IEvent> context);
    }
}
