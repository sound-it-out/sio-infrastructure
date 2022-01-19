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

            source.AddEntityFrameworkCoreStore(o => IntializeStoreOptions(sqlBuilder, o));

            if (!string.IsNullOrWhiteSpace(sqlBuilder.ProjectionConnectionString))
            {
                source.Services.AddDbContext<SIOProjectionDbContext>(options =>
                {
                    options.UseSqlServer(sqlBuilder.ProjectionConnectionString, sqlBuilder.ProjectionOptions);
                });
            }

            return source;
        }

        private static void IntializeStoreOptions(SIOEntityFrameworkCoreSqlServerOptions builder, EntityFrameworkCoreStoreOptions options)
        {
            foreach (var storeOption in builder.StoreOptions)
                options.WithContext(storeOption.StoreType, o => o.UseSqlServer(storeOption.ConnectionString, storeOption.StoreOptions));
        }
    }
}
