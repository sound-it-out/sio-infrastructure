using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SIO.Infrastructure.Files
{
    public interface IFileClient
    {
        Task UploadAsync(string fileName, string userId, Stream stream, CancellationToken cancellationToken = default);
        Task DownloadAsync(string fileName, string userId, Stream stream, CancellationToken cancellationToken = default);
        Task DeleteAsync(string fileName, string userId, CancellationToken cancellationToken = default);
    }
}
