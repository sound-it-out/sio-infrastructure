using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface IEventSerializer
    {
        string Serialize<T>(T data);
    }
}
