using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;

namespace SIO.Infrastructure.EntityFrameworkCore.SqlServer.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddEntityFrameworkCoreSqlServer(this ISIOInfrastructureBuilder source, Action<SIOEntityFrameworkCoreSqlServerOptions> builderAction)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.AddEntityFrameworkCore();

            var sqlBuilder = new SIOEntityFrameworkCoreSqlServerOptions();
            builderAction(sqlBuilder);

            if (!string.IsNullOrWhiteSpace(sqlBuilder.StoreConnectionString))
            {
                source.Services.AddDbContext<SIOStoreDbContext>(options =>
                {
                    options.UseSqlServer(sqlBuilder.StoreConnectionString, sqlBuilder.StoreOptions);
                });
            }

            if (!string.IsNullOrWhiteSpace(sqlBuilder.ProjectionConnectionString))
            {
                source.Services.AddDbContext<SIOProjectionDbContext>(options =>
                {
                    options.UseSqlServer(sqlBuilder.ProjectionConnectionString, sqlBuilder.ProjectionOptions);
                });
            }

            return source;
        }
    }
}
