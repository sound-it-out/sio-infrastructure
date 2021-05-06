using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.EntityFrameworkCore.Projections;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;
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
    }
}
