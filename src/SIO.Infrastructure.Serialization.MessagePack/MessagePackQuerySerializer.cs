using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    internal sealed class MessagePackQuerySerializer : IQuerySerializer
    {
        private readonly MessagePackSerializerOptions _options;
        private readonly ILogger<MessagePackQuerySerializer> _logger;

        public MessagePackQuerySerializer(MessagePackSerializerOptions options,
            ILogger<MessagePackQuerySerializer> logger)
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
                _logger.LogInformation($"{nameof(MessagePackQuerySerializer)}.{nameof(Serialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.Serialize(data, _options, cancellationToken: cancellationToken);
        }

        public string SerializeToJson<T>(T data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackQuerySerializer)}.{nameof(SerializeToJson)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return MessagePackSerializer.SerializeToJson(data, _options, cancellationToken: cancellationToken);
        }
    }
}
