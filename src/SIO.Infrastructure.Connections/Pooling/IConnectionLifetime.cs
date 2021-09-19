using System;
using System.Threading;

namespace SIO.Infrastructure.Connections.Pooling
{
    public interface IConnectionLifetime<TConnection> : IDisposable
        where TConnection : IConnection
    {
        TConnection OpenConnection(CancellationToken cancellationToken = default);
    }
}
