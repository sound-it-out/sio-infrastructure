using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Files;

namespace SIO.Infrastructure.Azure.Storage.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddAzureStorage(this ISIOInfrastructureBuilder builder, Action<AzureStorageBlobOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));


            builder.Services.Configure(options);
            builder.Services.AddScoped<IFileClient, AzureFileClient>();

            return builder;
        }
    }
}
