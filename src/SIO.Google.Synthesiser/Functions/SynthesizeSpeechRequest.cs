namespace SIO.Google.Synthesiser.Functions
{
    internal sealed class SynthesizeSpeechRequest
    {
        public int Sequence { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public int Version { get; set; }
    }
}
