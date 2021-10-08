using System;
using System.Collections.Generic;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Extensions
{
    public class EventOptions
    {
        internal HashSet<Type> Events { get; }

        public EventOptions()
        {
            Events = new HashSet<Type>();
        }

        public void Register<T>() 
            where T : IEvent
        {
            Events.Add(typeof(T));
        }

        public void Register(params Type[] types)
        {
            foreach(var type in types)
                Events.Add(type);
        }
    }
}
