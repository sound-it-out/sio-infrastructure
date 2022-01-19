using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    internal sealed class SIOStoreDbContextFactory<TStoreDbContext> : ISIOStoreDbContextFactory<TStoreDbContext>
        where TStoreDbContext : DbContext, ISIOStoreDbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public SIOStoreDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public TStoreDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<TStoreDbContext>>();
            return (TStoreDbContext)Activator.CreateInstance(typeof(TStoreDbContext), new object[] { options });
        }
    }
}
