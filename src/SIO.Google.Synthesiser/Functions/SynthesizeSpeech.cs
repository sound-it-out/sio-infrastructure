using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SIO.Domain.Translations.Events;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Files;

namespace SIO.Google.Synthesiser.Functions
{
    public class SynthesizeSpeech
    {
        public const string Name = "sio-google-synthesiser-synthesize-speech";
        private readonly ILogger<SynthesizeSpeech> _logger;
        private readonly IFileClient _fileClient;
        private readonly IEventManager _eventManager;
        private readonly TextToSpeechClient _client;

        public SynthesizeSpeech(IOptions<GoogleCredentialOptions> googleCredentialOptions)
        {
            if (googleCredentialOptions == null)
                throw new ArgumentNullException(nameof(googleCredentialOptions));

            var credentials = GoogleCredential.FromJson(JsonConvert.SerializeObject(googleCredentialOptions.Value));

            var builder = new TextToSpeechClientBuilder();
            builder.ChannelCredentials = credentials.ToChannelCredentials();
            _client = builder.Build();
        }

        [FunctionName(Name)]
        public async Task ExecuteAsync([OrchestrationTrigger] IDurableOrchestrationContext context, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProcessText)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var request = context.GetInput<SynthesizeSpeechRequest>();

            var response = await _client.SynthesizeSpeechAsync(
                input: new SynthesisInput
                {
                    Text = request.Text
                },
                voice: new VoiceSelectionParams
                    {
                        Name = ""
                    },
                audioConfig: new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3
                }
            );

            var fileName = $"{request.Subject}_{request.Version}.mp3";

            using (var ms = new MemoryStream(response.AudioContent.ToByteArray()))
            {
                await _fileClient.UploadAsync(fileName, request.UserId, ms);
            }

            var translationCharactersProcessed = new TranslationCharactersProcessed(request.Subject, request.Version, fileName, request.Text.Length);
            await _eventManager.ProcessAsync(translationCharactersProcessed);
        }
    }
}
