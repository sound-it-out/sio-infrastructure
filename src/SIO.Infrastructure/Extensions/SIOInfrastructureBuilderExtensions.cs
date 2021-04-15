using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Commands;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Queries;

namespace SIO.Infrastructure.Serialization.MessagePack.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddCommands(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ICommandDispatcher, DefaultCommandDispatcher>();

            return builder;
        }
        public static ISIOInfrastructureBuilder AddQueries(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IQueryDispatcher, DefaultQueryDispatcher>();

            return builder;
        }
        public static ISIOInfrastructureBuilder AddEvents(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IEventDispatcher, DefaultEventDispatcher>();
            builder.Services.AddScoped<IEventManager, DefaultEventManager>();

            return builder;
        }
    }
}
