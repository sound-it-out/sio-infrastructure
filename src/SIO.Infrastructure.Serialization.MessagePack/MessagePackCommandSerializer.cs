using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    internal sealed class MessagePackCommandSerializer : ICommandSerializer
    {
        private readonly ILogger<MessagePackCommandSerializer> _logger;

        public MessagePackCommandSerializer(ILogger<MessagePackCommandSerializer> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public byte[] Serialize<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandSerializer)}.{nameof(Serialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.Serialize(data, cancellationToken: cancellationToken);
        }

        public string SerializeToJson<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandSerializer)}.{nameof(SerializeToJson)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.SerializeToJson(data, cancellationToken: cancellationToken);
        }
    }
}
