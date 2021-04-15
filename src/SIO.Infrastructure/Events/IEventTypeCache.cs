using System;

namespace SIO.Infrastructure.Events
{
    public interface IEventTypeCache
    {
        bool TryGet(string name, out Type type);
    }
}
