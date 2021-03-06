using System;
using System.Text;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Serialization;

namespace SIO.Infrastructure.RabbitMQ.Messages
{
    internal sealed class DefaultMessageFactory : IMessageFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public DefaultMessageFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));
          
            _eventSerializer = eventSerializer;
        }

        public Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = typeof(TEvent).Name;

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = context.Payload.GetType().Name;

            return CreateMessage(eventName, context);
        }

        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _eventSerializer.Serialize(context.Payload);

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = context.CorrelationId,
                CausationId = context.CausationId,
                UserId = context.UserId,
                Body = Encoding.UTF8.GetBytes(body),
            };
        }
    }
}
