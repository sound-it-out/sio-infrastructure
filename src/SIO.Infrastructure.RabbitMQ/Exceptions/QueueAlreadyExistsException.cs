using System;

namespace SIO.Infrastructure.RabbitMQ.Exceptions
{
    public class QueueAlreadyExistsException : Exception
    {
        public string QueueName { get; }

        public QueueAlreadyExistsException(string name)
            : base($"A queue with name '{name}' already exists.")
        {
            QueueName = name;
        }

    }
}
