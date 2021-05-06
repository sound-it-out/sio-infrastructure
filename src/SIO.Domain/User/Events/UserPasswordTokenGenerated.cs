using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserPasswordTokenGenerated : Event
    {
        [Key(nameof(Token))]
        public string Token { get; set; }

        public UserPasswordTokenGenerated(string subject, int version) : base(subject, version)
        {
        }
    }
}
