using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

            if (sqlBuilder.UseStore)
            {
                source.Services.AddDbContext<SIOStoreDbContext>((sp, options) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("Store");

                    options.UseSqlServer(connectionString, sqlBuilder.StoreOptions);
                });
            }

            if (sqlBuilder.UseProjections)
            {
                source.Services.AddDbContext<SIOProjectionDbContext>((sp, options) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("Projection");

                    options.UseSqlServer(connectionString, sqlBuilder.ProjectionOptions);
                });
            }

            return source;
        }
    }
}
