using System;
using System.Threading;

namespace SIO.Infrastructure.Serialization
{
    public interface ICommandDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
