using System.Threading;

namespace SIO.Infrastructure.Connections.Pooling
{
    public interface IConnectionPool<TConnection>
        where TConnection: IConnection
    {
        IConnectionLifetime<TConnection> CreateLifetime();
        TConnection GetConnection(string connectionId, CancellationToken cancellationToken = default);
        void ReleaseConnection(string connectionId);
    }
}
