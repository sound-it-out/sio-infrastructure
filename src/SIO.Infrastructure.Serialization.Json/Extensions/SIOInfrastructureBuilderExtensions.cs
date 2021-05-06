using System;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure.Serialization.Json.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddJsonSerializers(this ISIOInfrastructureBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ICommandDeserializer, JsonCommandDeserializer>();
            builder.Services.AddSingleton<ICommandSerializer, JsonCommandSerializer>();
            builder.Services.AddSingleton<IEventDeserializer, JsonEventDeserializer>();
            builder.Services.AddSingleton<IEventSerializer, JsonEventSerializer>();
            builder.Services.AddSingleton<IQueryDeserializer, JsonQueryDeserializer>();
            builder.Services.AddSingleton<IQuerySerializer, JsonQuerySerializer>();

            return builder;
        }
    }
}
