using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.RabbitMQ.Connections
{
    public interface IRabbitMqConnectionFactory
    {
        Task<IRabbitMqConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
}
