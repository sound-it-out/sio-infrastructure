using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserEmailChanged : Event
    {
        [Key(nameof(Email))]
        public string Email { get; set; }

        public UserEmailChanged(string subject, int version) : base(subject, version)
        {
        }
    }
}
