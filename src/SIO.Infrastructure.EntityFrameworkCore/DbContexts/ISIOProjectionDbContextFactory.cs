using Microsoft.EntityFrameworkCore.Design;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public interface ISIOProjectionDbContextFactory
    {
        SIOProjectionDbContext Create();
    }
}
