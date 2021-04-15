using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIO.Infrastructure.RabbitMQ.Management.Api
{
    public interface IRabbitMqManagementApiClient
    {
        Task<IEnumerable<RabbitMqBinding>> RetrieveSubscriptionsAsync(string queue);
    }
}
