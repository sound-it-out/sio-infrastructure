using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Entities.Event dbEvent);
    }
}
