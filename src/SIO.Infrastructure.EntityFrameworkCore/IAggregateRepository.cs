using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Infrastructure.EntityFrameworkCore
{
    public interface IEntityFrameworkAggregateRepository<TStoreContext> : IAggregateRepository<TStoreContext>
        where TStoreContext : DbContext, ISIOStoreDbContext
    {
    }
}
