namespace SIO.Infrastructure.Events
{
    public abstract class UserEvent : Event
    {
        public string User { get; }

        protected UserEvent(string subject, int version, string user) : base(subject, version)
        {
            User = user;
        }
    }
}
