using System;

namespace SIO.Infrastructure.RabbitMQ.Exceptions
{
    public class ExchangeAlreadyExistsException : Exception
    {
        public string ExchangeName { get; }

        public ExchangeAlreadyExistsException(string name)
            : base($"An exchange with name '{name}' already exists.")
        {
            ExchangeName = name;
        }

    }
}
