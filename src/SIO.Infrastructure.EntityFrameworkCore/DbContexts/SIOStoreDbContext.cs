using Microsoft.EntityFrameworkCore;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public class SIOStoreDbContext : SIOStoreDbContextBase<SIOStoreDbContext>
    {
        public SIOStoreDbContext(DbContextOptions<SIOStoreDbContext> options)
            : base(options) { }
    }
}
