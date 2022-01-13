using Microsoft.EntityFrameworkCore;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public class SIOStoreDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Query> Queries { get; set; }

        public SIOStoreDbContext(DbContextOptions<SIOStoreDbContext> options)
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
