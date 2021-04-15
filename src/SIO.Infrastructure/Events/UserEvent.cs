using MessagePack;

namespace SIO.Infrastructure.Events
{
    [MessagePackObject]
    public abstract class UserEvent : Event
    {
        [Key(nameof(User))]
        public string User { get; }

        protected UserEvent(string subject, int version, string user) : base(subject, version)
        {
            User = user;
        }
    }
}
