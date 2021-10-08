using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace SIO.Infrastructure.Azure.ServiceBus.Topics
{
    public interface ITopicClientFactory
    {
        Task<ITopicClient> CreateAsync();
    }
}
