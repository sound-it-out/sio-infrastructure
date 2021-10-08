using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Translations.Events
{
    [MessagePackObject]
    public class TranslationCharactersProcessed : Event
    {
        [Key(nameof(CharactersProcessed))]
        public long CharactersProcessed { get; set; }
        [Key(nameof(FileName))]
        public string FileName { get; set; }

        public TranslationCharactersProcessed(string subject, int version, string fileName, long charactersProcessed) : base(subject, version)
        {
            FileName = fileName;
            CharactersProcessed = charactersProcessed;
        }
    }
}
