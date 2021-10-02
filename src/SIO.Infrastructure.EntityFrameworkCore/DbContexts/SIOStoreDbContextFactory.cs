using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    internal sealed class SIOStoreDbContextFactory : ISIOStoreDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SIOStoreDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public SIOStoreDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<SIOStoreDbContext>>();
            return new SIOStoreDbContext(options);
        }
    }
}
