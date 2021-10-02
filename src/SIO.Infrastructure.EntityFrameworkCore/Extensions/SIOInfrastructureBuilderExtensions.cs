using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using SIO.EntityFrameworkCore.Projections;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
using SIO.Infrastructure.EntityFrameworkCore.Projections;
using SIO.Infrastructure.EntityFrameworkCore.Stores;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Projections;
using SIO.Infrastructure.Queries;

namespace SIO.Infrastructure.EntityFrameworkCore.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddEntityFrameworkCore(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IAggregateRepository, AggregateRepository>();
            builder.Services.AddScoped<IEventStore, EntityFrameworkCoreEventStore>();
            builder.Services.AddScoped<ICommandStore, EntityFrameworkCoreCommandStore>();
            builder.Services.AddScoped<IQueryStore, EntityFrameworkCoreQueryStore>();
            builder.Services.AddScoped<IEventContextFactory, DefaultEventContextFactory>();
            builder.Services.AddScoped<IEventModelFactory, DefaultEventModelFactory>();            
            builder.Services.AddScoped<ISIOStoreDbContextFactory, SIOStoreDbContextFactory>();
            builder.Services.AddScoped<ISIOProjectionDbContextFactory, SIOProjectionDbContextFactory>();
            builder.Services.AddScoped(typeof(IProjectionWriter<>), typeof(EntityFrameworkCoreProjectionWriter<>));

            return builder;
        }

        public static ISIOInfrastructureBuilder AddEntityFrameworkCoreStoreProjector<TProjection>(this ISIOInfrastructureBuilder builder, Action<StoreProjectorOptions<TProjection>> options = null)
            where TProjection : class, IProjection
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            options ??= o => o.Interval = 1000;

            builder.Services.Configure(options);
            builder.Services.AddSingleton<IProjector<TProjection>, EntityFrameworkCoreStoreProjector<TProjection>>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<IProjector<TProjection>>());

            return builder;
        }
    }
}
