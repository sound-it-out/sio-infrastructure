using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.EntityFrameworkCore.Stores
{
    public interface IEntityFrameworkEventStore<TStoreContext> : IEventStore<TStoreContext>
        where TStoreContext : DbContext, ISIOStoreDbContext 
    {
    }
}
