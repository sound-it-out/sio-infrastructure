using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private static Lazy<MethodInfo> _registerProjectionMethod = new Lazy<MethodInfo>(() => typeof(SIOInfrastructureBuilderExtensions)
            .GetMethod(nameof(SIOInfrastructureBuilderExtensions.InternalRegisterProjection), BindingFlags.Static | BindingFlags.NonPublic));

        private static void InternalRegisterProjection<TProjection>(IServiceCollection services, object options)
            where TProjection : class, IProjection
        {
            services.Configure((Action<StoreProjectorOptions<TProjection>>)options);
            services.AddSingleton<IProjector<TProjection>, EntityFrameworkCoreStoreProjector<TProjection>>();
            services.AddScoped<IProjectionWriter<TProjection>, EntityFrameworkCoreProjectionWriter<TProjection>>();
            services.AddHostedService(sp => sp.GetRequiredService<IProjector<TProjection>>());
        }

        private static void RegisterProjection(this IServiceCollection services, Type type, object options)
            => _registerProjectionMethod.Value.MakeGenericMethod(type).Invoke(null, new object[] { services, options });

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

            return builder;
        }

        public static ISIOInfrastructureBuilder AddEntityFrameworkCoreStoreProjector(this ISIOInfrastructureBuilder builder, Action<EntityFrameworkCoreStoreProjectorOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var entityFrameworkCoreStoreProjectorOptions = new EntityFrameworkCoreStoreProjectorOptions();
            options(entityFrameworkCoreStoreProjectorOptions);

            foreach (var (type, projectorOptions) in entityFrameworkCoreStoreProjectorOptions.Projections)
                builder.Services.RegisterProjection(type, projectorOptions);

            return builder;
        }
    }

    public class EntityFrameworkCoreStoreProjectorOptions
    {
        internal List<(Type type, object options)> Projections { get; }

        public EntityFrameworkCoreStoreProjectorOptions()
        {
            Projections = new();
        }

        public void WithProjection<TProjection>(Action<StoreProjectorOptions<TProjection>> options = null)
            where TProjection : class, IProjection
        {
            options ??= o => o.Interval = 1000;
            Projections.Add((
                type: typeof(TProjection),
                options: options
            ));
        }
    }
}
