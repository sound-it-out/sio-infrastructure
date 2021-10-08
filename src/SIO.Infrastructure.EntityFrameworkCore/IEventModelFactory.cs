using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    public interface IEventModelFactory
    {
        Entities.Event Create(string streamId, IEventContext<IEvent> context);
    }
}
