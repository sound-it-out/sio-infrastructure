using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Translations.Events
{
    [MessagePackObject]
    public class TranslationFailed : Event
    {
        [Key(nameof(Error))]
        public string Error { get; set; }

        public TranslationFailed(string subject, int version) : base(subject, version)
        {
        }
    }
}
