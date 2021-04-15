using System;
using MessagePack;

namespace SIO.Infrastructure.Events
{
    [MessagePackObject]
    public abstract class Event : IEvent
    {
        [Key(nameof(Id))]
        public EventId Id { get; protected set; }
        [Key(nameof(Subject))]
        public string Subject { get; protected set; }
        [Key(nameof(Timestamp))]
        public DateTimeOffset Timestamp { get; protected set; }
        [Key(nameof(Version))]
        public int Version { get; protected set; }

        public Event(string subject, int version)
        {
            Id = EventId.New();
            Subject = subject;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
        }
    }
}
