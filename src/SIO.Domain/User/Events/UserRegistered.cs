using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserRegistered : Event
    {
        [Key(nameof(Email))]
        public string Email { get; set; }
        [Key(nameof(FirstName))]
        public string FirstName { get; set; }
        [Key(nameof(LastName))]
        public string LastName { get; set; }
        [Key(nameof(ActivationToken))]
        public string ActivationToken { get; set; }

        public UserRegistered(string subject, int version) : base(subject, version)
        {
        }
    }
}
