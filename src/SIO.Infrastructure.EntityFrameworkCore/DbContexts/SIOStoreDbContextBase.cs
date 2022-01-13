using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;
using Event = SIO.Infrastructure.EntityFrameworkCore.Entities.Event;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public class SIOStoreDbContextBase<TStoreDbContext> : DbContext, ISIOStoreDbContext
        where TStoreDbContext : DbContext, ISIOStoreDbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Query> Queries { get; set; }

        public SIOStoreDbContextBase(DbContextOptions<TStoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CommandEntityTypeConfiguration());
            builder.ApplyConfiguration(new EventEntityTypeConfiguration());
            builder.ApplyConfiguration(new QueryEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
