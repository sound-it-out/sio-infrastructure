using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Translations.Events
{
    [MessagePackObject]
    public class TranslationStarted : Event
    {
        [Key(nameof(CharacterCount))]
        public long CharacterCount { get; set; }
        [Key(nameof(ProcessCount))]
        public int ProcessCount { get; set; }

        public TranslationStarted(string subject, int version, long characterCount, int processCount) : base(subject, version)
        {
            CharacterCount = characterCount;
            ProcessCount = processCount;
        }
    }
}
