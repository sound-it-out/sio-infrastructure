using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserVerified : Event
    {
        public UserVerified(string subject, int version) : base(subject, version)
        {
        }
    }
}
