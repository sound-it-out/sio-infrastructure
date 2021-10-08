using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    internal sealed class MessagePackCommandDeserializer : ICommandDeserializer
    {
        private readonly MessagePackSerializerOptions _options;
        private readonly ILogger<MessagePackCommandDeserializer> _logger;

        public MessagePackCommandDeserializer(MessagePackSerializerOptions options,
            ILogger<MessagePackCommandDeserializer> logger)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _options = options;
            _logger = logger;
        }

        public object Deserialize(byte[] data, Type type, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return MessagePackSerializer.Deserialize(type, data, _options, cancellationToken: cancellationToken);
        }

        public T Deserialize<T>(byte[] data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return MessagePackSerializer.Deserialize<T>(data, _options, cancellationToken: cancellationToken);
        }

        public object DeserializeFromJson(string data, Type type, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return Deserialize(MessagePackSerializer.ConvertFromJson(data, _options, cancellationToken: cancellationToken), type, cancellationToken: cancellationToken);
        }

        public T DeserializeFromJson<T>(string data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackCommandDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return Deserialize<T>(MessagePackSerializer.ConvertFromJson(data, _options, cancellationToken: cancellationToken), cancellationToken: cancellationToken);
        }
    }
}
