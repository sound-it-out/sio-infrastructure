using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface IProjectionSerializer
    {
        string Serialize<T>(T data);
    }
}
