using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface IQuerySerializer
    {
        string Serialize<T>(T data);
    }
}
