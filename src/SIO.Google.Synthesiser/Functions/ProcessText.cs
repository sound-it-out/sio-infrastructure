using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clipboard;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SIO.Domain.Translations.Events;
using SIO.Google.Synthesiser.Extensions;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Files;
using SIO.Infrastructure.Extensions;
using System.Collections.Generic;

namespace SIO.Google.Synthesiser.Functions
{
    public class ProcessText
    {
        public const string Name = "sio-google-synthesiser-process-text";
        private readonly IFileClient _fileClient;
        private readonly IEventManager _eventManager;
        private readonly ILogger<ProcessText> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public ProcessText(IFileClient fileClient,
            IEventManager eventManager,
            ILogger<ProcessText> logger)
        {
            if (fileClient == null)
                throw new ArgumentNullException(nameof(fileClient));
            if (eventManager == null)
                throw new ArgumentNullException(nameof(eventManager));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _fileClient = fileClient;
            _eventManager = eventManager;
            _logger = logger;
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [FunctionName(Name)]
        public async Task ExecuteAsync([OrchestrationTrigger] IDurableOrchestrationContext context, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProcessText)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            var request = context.GetInput<ProcessTextRequest>();

            using(var ms = new MemoryStream())
            {
                await _fileClient.DownloadAsync(request.FileName, request.UserId, ms, cancellationToken);
                ms.Position = 0;

                if (!_fileExtensionContentTypeProvider.TryGetContentType(request.FileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                using (var textExtractor = TextExtractor.Open(ms, contentType))
                {
                    var text = await textExtractor.ExtractAsync();
                    var chunks = text.ChunkWithDelimeters(5000, '.', '!', '?', ')', '"', '}', ']').ToArray();

                    var translationStarted = new TranslationStarted(request.Subject, request.Version, chunks.Sum(c => c.Length), chunks.Length);

                    await _eventManager.ProcessAsync(translationStarted);

                    var index = 1;

                    foreach (var chunk in chunks.Chunk(30))
                    {
                        if(index > 1)
                            await Task.Delay(60000);

                        var tasks = new List<Task>();

                        foreach(var child in chunk)
                        {
                            tasks.Add(context.CallActivityAsync(SynthesizeSpeech.Name, new SynthesizeSpeechRequest
                            {
                                Subject = request.Subject,
                                Text = child,
                                UserId = request.UserId,
                                Version = request.Version + index
                            }));
                        }

                        await Task.WhenAll(tasks);

                        index++;
                    }                        
                }
            }
        }
    }
}
