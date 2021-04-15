using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    internal sealed class MessagePackEventSerializer : IEventSerializer
    {
        private readonly ILogger<MessagePackEventSerializer> _logger;

        public MessagePackEventSerializer(ILogger<MessagePackEventSerializer> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public byte[] Serialize<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackEventSerializer)}.{nameof(Serialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.Serialize(data, cancellationToken: cancellationToken);
        }

        public string SerializeToJson<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackEventSerializer)}.{nameof(SerializeToJson)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.SerializeToJson(data, cancellationToken: cancellationToken);
        }
    }
}
