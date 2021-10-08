using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.RabbitMQ.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(ReceivedMessage message);
    }
}
