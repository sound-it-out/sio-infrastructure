using Microsoft.Azure.ServiceBus;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Azure.ServiceBus.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Message message);
    }
}
