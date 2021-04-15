using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.RabbitMQ.Connections;
using SIO.Infrastructure.RabbitMQ.Management;
using SIO.Infrastructure.RabbitMQ.Management.Api;
using SIO.Infrastructure.RabbitMQ.Messages;
using SIO.Infrastructure.RabbitMQ.Queues;
using SIO.Infrastructure.RabbitMQ.Subscriptions;

namespace SIO.Infrastructure.RabbitMQ.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddRabbitMq(this ISIOInfrastructureBuilder builder, Action<RabbitMqOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));


            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<IEventBusPublisher, RabbitMqEventBus>();
            builder.Services.AddScoped<IEventBusConsumer, RabbitMqEventBus>();
            builder.Services.AddSingleton<RabbitMqConnectionPool>();
            builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            builder.Services.AddScoped<IQueueMessageSender, DefaultQueueMessageSender>();
            builder.Services.AddSingleton<IQueueMessageReceiver, DefaultQueueMessageReceiver>();
            builder.Services.AddScoped<ISubscriptionManager, DefaultSubscriptionManager>();
            builder.Services.AddScoped<IRabbitMqManagementClient, RabbitMqManagementClient>();
            builder.Services.AddHttpClient<IRabbitMqManagementApiClient, RabbitMqManagementApiClient>();
            builder.Services.AddSingleton<IEventContextFactory, DefaultEventContextFactory>();

            return builder;
        }
    }
}
