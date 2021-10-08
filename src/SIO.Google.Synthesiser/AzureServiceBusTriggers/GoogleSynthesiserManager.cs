using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Domain.Documents.Events;
using SIO.Domain.Translations.Events;
using SIO.Google.Synthesiser.Functions;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using SIO.Infrastructure.Events;

namespace SIO.Google.Synthesiser.AzureServiceBusTriggers
{
    class GoogleSynthesiserManager
    {
        public const string Name = "sio-google-synthesiser";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GoogleSynthesiserManager> _logger;

        public GoogleSynthesiserManager(IServiceProvider serviceProvider,
            IEventManager eventManager,
            ILogger<GoogleSynthesiserManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        [FunctionName(Name)]
        public async Task ExecuteAsync([ServiceBusTrigger("%Topic%", "%Subscription%", Connection = "AzureServiceBus")] Message message,
            [DurableClient] IDurableOrchestrationClient client, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(GoogleSynthesiserManager)}.{nameof(ExecuteAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using(var scope = _serviceProvider.CreateScope())
            {
                var eventContextFactory = scope.ServiceProvider.GetRequiredService<IEventContextFactory>();
                var eventManager = scope.ServiceProvider.GetRequiredService<IEventManager>();
                if (message.Label == nameof(DocumentUploaded))
                {
                    var context = (IEventContext<DocumentUploaded>)eventContextFactory.CreateContext(message);

                    if (context.Payload.TranslationType != TranslationType.Google)
                        return;

                    var translationQueuedEvent = new TranslationQueued(
                        subject: context.Payload.Subject,
                        version: context.Payload.Version + 1
                    );

                    await eventManager.ProcessAsync(translationQueuedEvent, cancellationToken);

                    await client.StartNewAsync(ProcessText.Name, new ProcessTextRequest
                    {
                        FileName = $"{translationQueuedEvent.Subject}{Path.GetExtension(context.Payload.FileName)}",
                        Subject = translationQueuedEvent.Subject,
                        UserId = context.Payload.User,
                        Version = translationQueuedEvent.Version + 1
                    });
                }
            }
        }
    }
}
