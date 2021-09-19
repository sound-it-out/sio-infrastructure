using System.Threading;

namespace SIO.Infrastructure.Connections.Pooling
{
    internal sealed class ConnectionLifetime<TConnection> : IConnectionLifetime<TConnection>
        where TConnection : IConnection
    {
        private readonly IConnectionPool<TConnection> _connectionPool;
        private readonly string _connectionId;

        public ConnectionLifetime(IConnectionPool<TConnection> connectionPool)
        {
            _connectionPool = connectionPool;
            _connectionId = ConnectionId.New();
        }

        public void Dispose()
        {
            _connectionPool.ReleaseConnection(_connectionId);
        }

        public TConnection OpenConnection(CancellationToken cancellationToken = default)
        {
            return _connectionPool.GetConnection(_connectionId, cancellationToken);
        }
    }
}
