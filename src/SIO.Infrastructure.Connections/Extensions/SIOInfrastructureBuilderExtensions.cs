using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Connections.Pooling;

namespace SIO.Infrastructure.Connections.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddConnectionPool<TConnection, TConnectionFactory>(this ISIOInfrastructureBuilder builder)
            where TConnection: IConnection
            where TConnectionFactory : class, IConnectionFactory<TConnection>
        {
            builder.Services.AddSingleton<IConnectionFactory<TConnection>, TConnectionFactory>();
            builder.Services.AddSingleton<IConnectionPool<TConnection>, ConnectionPool<TConnection>>();
            return builder;
        }
    }
}
