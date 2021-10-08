using System;
using System.Threading;
using System.Threading.Tasks;
using SIO.Infrastructure.RabbitMQ.Messages;

namespace SIO.Infrastructure.RabbitMQ.Queues
{
    public interface IQueueMessageReceiver
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task OnReceiveAsync(ReceivedMessage message, CancellationToken cancellationToken);
        Task OnErrorAsync(ReceivedMessage message, Exception ex);
    }
}
