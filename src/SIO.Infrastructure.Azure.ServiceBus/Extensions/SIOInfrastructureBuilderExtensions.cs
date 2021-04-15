using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SIO.Infrastructure.Azure.ServiceBus.Management;
using SIO.Infrastructure.Azure.ServiceBus.Messages;
using SIO.Infrastructure.Azure.ServiceBus.Subscriptions;
using SIO.Infrastructure.Azure.ServiceBus.Topics;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Azure.ServiceBus.Extensions
{
    public static class SIOInfrastructureBuilderExtensions
    {
        public static ISIOInfrastructureBuilder AddAzureServiceBus(this ISIOInfrastructureBuilder builder, Action<ServiceBusOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<IEventBusPublisher, AzureServiceBus>();
            builder.Services.AddScoped<IEventBusConsumer, AzureServiceBus>();
            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<ISubscriptionClientFactory, DefaultSubscriptionClientFactory>();
            builder.Services.AddScoped<ISubscriptionClientManager, DefaultSubscriptionClientManager>();
            builder.Services.AddScoped<ITopicClientFactory, DefaultTopicClientFactory>();
            builder.Services.AddSingleton<ITopicMessageReceiver, DefaultTopicMessageReceiver>();
            builder.Services.AddSingleton<IEventContextFactory, DefaultEventContextFactory>();
            builder.Services.AddScoped<ITopicMessageSender, DefaultTopicMessageSender>();
            builder.Services.AddScoped<IServiceBusManagementClient, ServiceBusManagementClient>();
            builder.Services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>();
                var connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.Value.ConnectionString);

                if (string.IsNullOrEmpty(connectionStringBuilder.EntityPath) && options.Value.Topic == null)
                    throw new InvalidOperationException($"Azure service bus connection string doesn't contain an entity path and 'UseTopic(...)' has not been called. Either include the entity path in the connection string or by calling 'UseTopic(...)' during startup when configuring Azure Service Bus.");

                if (options.Value.Topic != null)
                    connectionStringBuilder.EntityPath = options.Value.Topic.Name;

                if (!string.IsNullOrEmpty(connectionStringBuilder.EntityPath) && options.Value.Topic == null)
                    options.Value.UseTopic(t => t.WithName(connectionStringBuilder.EntityPath));

                return connectionStringBuilder;
            });
            builder.Services.AddScoped(sp =>
            {
                var connectionStringBuilder = sp.GetRequiredService<ServiceBusConnectionStringBuilder>();

                return new ManagementClient(connectionStringBuilder);
            });

            return builder;
        }
    }
}
