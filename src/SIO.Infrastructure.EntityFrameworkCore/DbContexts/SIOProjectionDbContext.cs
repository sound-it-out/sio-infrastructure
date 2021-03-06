using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using SIO.Infrastructure.EntityFrameworkCore.Entities;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    public class SIOProjectionDbContext : DbContext
    {
        public DbSet<ProjectionState> ProjectionStates { get; set; }

        public SIOProjectionDbContext(DbContextOptions<SIOProjectionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectionStateEntityTypeConfiguration());

            var types = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IProjectionTypeConfiguration<>)))
                .ToArray();

            foreach (var type in types)
                builder.ApplyConfiguration((dynamic)Activator.CreateInstance(type));

            base.OnModelCreating(builder);
        }
    }
}
