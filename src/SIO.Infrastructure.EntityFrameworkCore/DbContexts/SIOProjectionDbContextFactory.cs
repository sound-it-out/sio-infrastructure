using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.EntityFrameworkCore.DbContexts
{
    internal sealed class SIOProjectionDbContextFactory : ISIOProjectionDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SIOProjectionDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public SIOProjectionDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<SIOProjectionDbContext>>();
            return new SIOProjectionDbContext(options);
        }
    }
}
