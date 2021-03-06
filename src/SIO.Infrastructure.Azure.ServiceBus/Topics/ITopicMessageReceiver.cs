using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SIO.Infrastructure.Azure.ServiceBus.Topics
{
    public interface ITopicMessageReceiver
    {
        Task ReceiveAsync(ISubscriptionClient client, Message message, CancellationToken cancellationToken = default);
        Task OnErrorAsync(ExceptionReceivedEventArgs error);
    }
}
