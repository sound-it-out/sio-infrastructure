using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Commands
{
    public interface ICommandStore
    {
        Task SaveAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
