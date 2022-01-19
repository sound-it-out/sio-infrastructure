using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
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
        private static Lazy<MethodInfo> _registerProjectionMethod = new Lazy<MethodInfo>(() => typeof(SIOInfrastructureBuilderExtensions)
            .GetMethod(nameof(SIOInfrastructureBuilderExtensions.InternalRegisterProjection), BindingFlags.Static | BindingFlags.NonPublic));

        private static Lazy<MethodInfo> _registerStoreMethod = new Lazy<MethodInfo>(() => typeof(SIOInfrastructureBuilderExtensions)
            .GetMethod(nameof(SIOInfrastructureBuilderExtensions.InternalRegisterStore), BindingFlags.Static | BindingFlags.NonPublic));

        private static void InternalRegisterProjection<TProjection, TProjectionManager, TDbContextStore>(IServiceCollection services, object options)
            where TProjection : class, IProjection
            where TProjectionManager : class, IProjectionManager<TProjection>
            where TDbContextStore : DbContext, ISIOStoreDbContext
        {
            services.Configure((Action<StoreProjectorOptions<TProjection>>)options);
            services.AddSingleton<IProjector<TProjection>, EntityFrameworkCoreStoreProjector<TProjection, TDbContextStore>>();
            services.AddScoped<IProjectionWriter<TProjection>, EntityFrameworkCoreProjectionWriter<TProjection>>();
            services.AddScoped<IProjectionManager<TProjection>, TProjectionManager>();
            services.AddHostedService(sp => sp.GetRequiredService<IProjector<TProjection>>());
        }

        private static void InternalRegisterStore<TStoreContext>(IServiceCollection services, object options)
            where TStoreContext : DbContext, ISIOStoreDbContext
        {
            services.AddDbContext<TStoreContext>((Action<DbContextOptionsBuilder>)options);
            services.AddScoped<IAggregateRepository<TStoreContext>, EntityFrameworkAggregateRepository<TStoreContext>>();
            services.AddScoped<IEventStore<TStoreContext>, EntityFrameworkCoreEventStore<TStoreContext>>();
            services.AddScoped<ISIOStoreDbContextFactory<TStoreContext>, SIOStoreDbContextFactory<TStoreContext>>();
        }

        private static void RegisterProjection(this IServiceCollection services, Type projectionType, Type managerType, Type storeType, object options)
            => _registerProjectionMethod.Value.MakeGenericMethod(projectionType, managerType, storeType).Invoke(null, new object[] { services, options });

        private static void RegisterStore(this IServiceCollection services, Type storeType, object options)
            => _registerStoreMethod.Value.MakeGenericMethod(storeType).Invoke(null, new object[] { services, options });

        public static ISIOInfrastructureBuilder AddEntityFrameworkCore(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ICommandStore, EntityFrameworkCoreCommandStore>();
            builder.Services.AddScoped<IQueryStore, EntityFrameworkCoreQueryStore>();
            builder.Services.AddScoped<IEventContextFactory, DefaultEventContextFactory>();
            builder.Services.AddScoped<IEventModelFactory, DefaultEventModelFactory>();
            builder.Services.AddScoped<ISIOProjectionDbContextFactory, SIOProjectionDbContextFactory>();            

            return builder;
        }

        public static ISIOInfrastructureBuilder AddEntityFrameworkCoreStoreProjector(this ISIOInfrastructureBuilder builder, Action<EntityFrameworkCoreStoreProjectorOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var entityFrameworkCoreStoreProjectorOptions = new EntityFrameworkCoreStoreProjectorOptions();
            options(entityFrameworkCoreStoreProjectorOptions);

            foreach (var (projectionType, managerType, storeType, projectorOptions) in entityFrameworkCoreStoreProjectorOptions.Projections)
                builder.Services.RegisterProjection(projectionType, managerType, storeType, projectorOptions);

            return builder;
        }

        public static ISIOInfrastructureBuilder AddEntityFrameworkCoreStore(this ISIOInfrastructureBuilder builder, Action<EntityFrameworkCoreStoreOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var entityFrameworkCoreStoreOptions = new EntityFrameworkCoreStoreOptions();
            options(entityFrameworkCoreStoreOptions);

            foreach (var (storeType, projectorOptions) in entityFrameworkCoreStoreOptions.Stores)
                builder.Services.RegisterStore(storeType, projectorOptions);

            return builder;
        }
    }

    public class EntityFrameworkCoreStoreProjectorOptions
    {
        internal List<(Type projectionType, Type managerType, Type storeType, object options)> Projections { get; }

        public EntityFrameworkCoreStoreProjectorOptions()
        {
            Projections = new();
        }

        public EntityFrameworkCoreStoreProjectorOptions WithProjection<TProjection, TProjectionManager, TStoreDbContext>(Action<StoreProjectorOptions<TProjection>> options = null)
            where TProjection : class, IProjection
            where TProjectionManager : class, IProjectionManager<TProjection>
            where TStoreDbContext : DbContext, ISIOStoreDbContext
        {
            options ??= o => o.Interval = 1000;

            Projections.Add((
                projectionType: typeof(TProjection),
                managerType: typeof(TProjectionManager),
                storeType: typeof(TStoreDbContext),
                options: options
            ));

            return this;
        }
    }

    public class EntityFrameworkCoreStoreOptions
    {
        internal List<(Type storeType, object options)> Stores { get; }

        public EntityFrameworkCoreStoreOptions()
        {
            Stores = new();
        }

        public EntityFrameworkCoreStoreOptions WithContext<TStoreContext>(Action<DbContextOptionsBuilder> options = null)
            where TStoreContext : DbContext, ISIOStoreDbContext
                => WithContext(typeof(TStoreContext), options);

        public EntityFrameworkCoreStoreOptions WithContext(Type storeType, Action<DbContextOptionsBuilder> options = null)
        {
            Stores.Add((
                storeType: storeType,
                options: options
            ));

            return this;
        }
    }
}
