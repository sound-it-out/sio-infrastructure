using System;
using System.Threading;
using MessagePack;
using Microsoft.Extensions.Logging;

namespace SIO.Infrastructure.Serialization.MessagePack
{
    internal sealed class MessagePackQueryDeserializer : IQueryDeserializer
    {
        private readonly ILogger<MessagePackQueryDeserializer> _logger;

        public MessagePackQueryDeserializer(ILogger<MessagePackQueryDeserializer> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public object Deserialize(byte[] data, Type type, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackQueryDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return MessagePackSerializer.Deserialize(type, data, cancellationToken: cancellationToken);
        }

        public T Deserialize<T>(byte[] data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackQueryDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return MessagePackSerializer.Deserialize<T>(data, cancellationToken: cancellationToken);
        }

        public object DeserializeFromJson(string data, Type type, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackQueryDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return Deserialize(MessagePackSerializer.ConvertFromJson(data, cancellationToken: cancellationToken), type, cancellationToken: cancellationToken);
        }

        public T DeserializeFromJson<T>(string data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(MessagePackQueryDeserializer)}.{nameof(Deserialize)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            return Deserialize<T>(MessagePackSerializer.ConvertFromJson(data, cancellationToken: cancellationToken), cancellationToken: cancellationToken);
        }
    }
}
