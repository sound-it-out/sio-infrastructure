using System.Collections.Generic;

namespace SIO.Infrastructure.Connections.Pooling
{
    public interface IConnectionFactory<TConnection>
        where TConnection: IConnection
    {
        IEnumerable<TConnection> CreateConnections();
    }
}
