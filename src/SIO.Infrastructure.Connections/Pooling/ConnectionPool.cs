using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace SIO.Infrastructure.Connections.Pooling
{
    internal class ConnectionPool<TConnection> : IConnectionPool<TConnection>
        where TConnection : IConnection
    {
        private readonly BlockingQueue<TConnection> _availableConnections;
        private readonly ConcurrentDictionary<ConnectionId, TConnection> _scopedConnections;
        private readonly object _scopedConnectionsLock = new object();
        private readonly object _avaliableConnectionsLock = new object();

        public ConnectionPool(IConnectionFactory<TConnection> connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _availableConnections = new BlockingQueue<TConnection>(connectionFactory.CreateConnections().ToArray());
            _scopedConnections = new ConcurrentDictionary<ConnectionId, TConnection>();
        }

        public IConnectionLifetime<TConnection> CreateLifetime() => new ConnectionLifetime<TConnection>(this);

        public TConnection GetConnection(string connectionId, CancellationToken cancellationToken = default)
        {
            lock (_scopedConnectionsLock)
            {
                if (_scopedConnections.TryGetValue(ConnectionId.From(connectionId), out var connection))
                    return connection;
            }

            lock (_avaliableConnectionsLock)
            {
                if (_availableConnections.TryDequeue(out var connection, cancellationToken))
                {
                    lock (_scopedConnectionsLock)
                    {
                        if (_scopedConnections.TryAdd(ConnectionId.From(connectionId), connection))
                        {
                            return connection;
                        }
                        else
                        {
                            _availableConnections.Enqueue(connection);
                            return GetConnection(connectionId);
                        }
                    }
                }
            }

            throw new InvalidOperationException("Unable to get a connection");
        }

        public void ReleaseConnection(string connectionId)
        {
            lock (_scopedConnectionsLock)
            {
                if (_scopedConnections.TryRemove(ConnectionId.From(connectionId), out var connection))
                {
                    lock (_avaliableConnectionsLock)
                    {
                        _availableConnections.Enqueue(connection);
                    }
                }
            }
        }
    }
}
