using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface IEventSerializer
    {
        byte[] Serialize<T>(T data, CancellationToken cancellationToken = default);
        string SerializeToJson<T>(T data, CancellationToken cancellationToken = default);
    }
}
