using System;
using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface ICommandDeserializer
    {
        object Deserialize(byte[] data, Type type, CancellationToken cancellationToken = default);
        object DeserializeFromJson(string data, Type type, CancellationToken cancellationToken = default);
        T Deserialize<T>(byte[] data, CancellationToken cancellationToken = default);
        T DeserializeFromJson<T>(string data, CancellationToken cancellationToken = default);
    }
}
