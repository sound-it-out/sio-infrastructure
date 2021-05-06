using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Serialization;

namespace SIO.Infrastructure.Azure.ServiceBus.Messages
{
    internal sealed class DefaultMessageFactory : IMessageFactory
    {
        private readonly IEventSerializer _eventSerializer;
        private readonly IEventDeserializer _eventDeserializer;

        public DefaultMessageFactory(IEventSerializer eventSerializer, IEventDeserializer eventDeserializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
        }

        public Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, context);
        }
        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _eventSerializer.Serialize(@event);
            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = Encoding.UTF8.GetBytes(body),
                Label = eventName,
                CorrelationId = context.CorrelationId?.ToString(),
                UserProperties =
                {
                    { nameof(context.StreamId), context.StreamId?.ToString() },
                    { nameof(context.CausationId), context.CausationId?.ToString() },
                    { nameof(context.UserId), context.UserId },
                    { nameof(context.Timestamp), context.Timestamp },
                },
            };

            return message;
        }
    }
}
