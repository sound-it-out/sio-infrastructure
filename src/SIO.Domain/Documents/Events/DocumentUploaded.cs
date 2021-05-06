using MessagePack;
using SIO.Infrastructure.Events;

namespace SIO.Domain.Documents.Events
{
    [MessagePackObject]
    public class DocumentUploaded : UserEvent
    {
        [Key(nameof(TranslationType))]
        public TranslationType TranslationType { get; set; }
        [Key(nameof(TranslationSubject))]
        public string TranslationSubject { get; set; }
        [Key(nameof(FileName))]
        public string FileName { get; set; }

        public DocumentUploaded(string subject, int version, string user) : base(subject, version, user)
        {
        }
    }
}
