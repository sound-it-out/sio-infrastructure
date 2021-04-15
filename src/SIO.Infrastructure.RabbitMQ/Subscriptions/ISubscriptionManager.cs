using System.Threading.Tasks;

namespace SIO.Infrastructure.RabbitMQ.Subscriptions
{
    public interface ISubscriptionManager
    {
        Task ConfigureAsync();
    }
}
