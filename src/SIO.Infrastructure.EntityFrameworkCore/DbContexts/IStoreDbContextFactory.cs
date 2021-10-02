using Microsoft.EntityFrameworkCore.Design;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public interface ISIOStoreDbContextFactory
    {
        SIOStoreDbContext Create();
    }
}
