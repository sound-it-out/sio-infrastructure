using System;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.Serialization.MessagePack.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddMessagePackSerialization(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ICommandDeserializer, MessagePackCommandDeserializer>();
            builder.Services.AddScoped<ICommandSerializer, MessagePackCommandSerializer>();
            builder.Services.AddScoped<IEventDeserializer, MessagePackEventDeserializer>();
            builder.Services.AddScoped<IEventSerializer, MessagePackEventSerializer>();
            builder.Services.AddScoped<IQueryDeserializer, MessagePackQueryDeserializer>();
            builder.Services.AddScoped<IQuerySerializer, MessagePackQuerySerializer>();

            return builder;
        }
    }
}
