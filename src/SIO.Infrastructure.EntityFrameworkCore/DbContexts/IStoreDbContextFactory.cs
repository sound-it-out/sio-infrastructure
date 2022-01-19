using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public interface ISIOStoreDbContextFactory<TStoreDbContext>
        where TStoreDbContext : DbContext, ISIOStoreDbContext
    {
        TStoreDbContext Create();
    }
}
