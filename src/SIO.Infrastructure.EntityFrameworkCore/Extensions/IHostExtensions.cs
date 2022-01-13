using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

namespace SIO.Infrastructure.EntityFrameworkCore.Extensions
{
    public static class IHostExtensions
    {
        public static async Task RunStoreMigrationsAsync<TStoreDbContext>(this IHost host)
            where TStoreDbContext : DbContext, ISIOStoreDbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<ISIOStoreDbContextFactory<TStoreDbContext>>().Create())
                    await context.Database.MigrateAsync();
            }
        }

        public static async Task RunProjectionMigrationsAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<ISIOProjectionDbContextFactory>().Create())
                    await context.Database.MigrateAsync();
            }
        }
    }
}
