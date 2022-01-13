using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SIO.Infrastructure.EntityFrameworkCore.Migrations
{
    public sealed class Migrator
    {
        public readonly List<(Type dbType, object options)> _contexts = new();

        private static Lazy<MethodInfo> _registerDbContextMethod = new Lazy<MethodInfo>(() => typeof(Migrator)
            .GetMethod(nameof(Migrator.InternalRegisterDbContext), BindingFlags.Static | BindingFlags.NonPublic));

        public Migrator WithDbContext<TDbContext>(Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext
        {
            _contexts.Add((
                dbType: typeof(TDbContext),
                options: options
            ));

            return this;
        }

        public async Task RunAsync(string[] args)
            => await CreateHostBuilder(args).Build().RunAsync();

        private IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    foreach (var dbContext in _contexts)
                        RegisterDbStore(services, dbContext.dbType, dbContext.options);
                }
            );

        private static void RegisterDbStore(IServiceCollection services, Type dbContextType, object options)
            => _registerDbContextMethod.Value.MakeGenericMethod(dbContextType).Invoke(null, new object[] { services, options });

        private static void InternalRegisterDbContext<TDbContext>(IServiceCollection services, object options)
            where TDbContext : DbContext
        {
            services.AddSingleton<IDesignTimeDbContextFactory<TDbContext>>(new DbContextFactory<TDbContext>((Action<DbContextOptionsBuilder>)options));
            services.AddDbContext<TDbContext>((Action<DbContextOptionsBuilder>)options);
        }
    }

    internal class DbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> _options;

        public DbContextFactory(Action<DbContextOptionsBuilder> options)
        {
            _options = options;
        }

        public TDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            _options(optionsBuilder);

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object[] { optionsBuilder.Options });
        }
    }
}
