using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SIO.Infrastructure.RabbitMQ.Connections
{
    internal class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly RabbitMqConnectionPool _pool;

        public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options,
                                         RabbitMqConnectionPool pool)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            _options = options;
            _pool = pool;
        }

        public async Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Yield();

            var connection = _pool.GetConnection();

            return connection;
        }
    }
}
