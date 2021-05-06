using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface ICommandSerializer
    {
        string Serialize<T>(T data);
    }
}
