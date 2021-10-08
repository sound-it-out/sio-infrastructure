using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    public sealed class MessagePackEventSerializer : IEventSerializer
    {
        private readonly MessagePackSerializerOptions _options;
        private readonly ILogger<MessagePackEventSerializer> _logger;

        public MessagePackEventSerializer(MessagePackSerializerOptions options,
            ILogger<MessagePackEventSerializer> logger)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _options = options;
            _logger = logger;
        }

        public byte[] Serialize<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackEventSerializer)}.{nameof(Serialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.Serialize(data, _options, cancellationToken: cancellationToken);
        }

        public string SerializeToJson<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackEventSerializer)}.{nameof(SerializeToJson)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.SerializeToJson(data, _options, cancellationToken: cancellationToken);
        }
    }
}
