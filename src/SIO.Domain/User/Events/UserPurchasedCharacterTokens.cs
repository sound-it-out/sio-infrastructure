using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.User.Events
{
    [MessagePackObject]
    public class UserPurchasedCharacterTokens : Event
    {
        [Key(nameof(CharacterTokens))]
        public long CharacterTokens { get; set; }

        public UserPurchasedCharacterTokens(string subject, int version) : base(subject, version)
        {
        }
    }
}
