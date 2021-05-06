using System;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
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

            var resolver = CompositeResolver.Create(
              new IMessagePackFormatter[] 
              { 
                  NativeDateTimeArrayFormatter.Instance, 
                  NativeDateTimeFormatter.Instance, 
                  NativeDecimalFormatter.Instance, 
                  NativeGuidFormatter.Instance,
                  TypelessFormatter.Instance,  
              },
              new IFormatterResolver[] 
              {
                  NativeDateTimeResolver.Instance, 
                  NativeDecimalResolver.Instance, 
                  NativeGuidResolver.Instance,
                  ContractlessStandardResolverAllowPrivate.Instance,
                  TypelessObjectResolver.Instance, 
                  StandardResolverAllowPrivate.Instance
              });

            var options = MessagePackSerializerOptions.Standard
                .WithCompression(MessagePackCompression.None)
                .WithResolver(resolver);

            builder.Services.AddSingleton(options);

            return builder;
        }
    }
}
