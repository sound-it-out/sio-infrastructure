using System;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Sdk;

namespace SIO.Infrastructure.Azure.ServiceBus.Tests
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [TraitDiscoverer("SIO.Infrastructure.Azure.ServiceBus.Tests.ServiceBusTestTraitDiscoverer", "SIO.Infrastructure.Azure.ServiceBus.Tests")]
    public class ServiceBusTestAttribute : FactAttribute, ITraitAttribute 
    {
        private readonly IConfiguration _configuration;

        public override string Skip { get; set; }

        public ServiceBusTestAttribute()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(ServiceBusTestAttribute).Assembly, optional: true)
                .AddEnvironmentVariables(prefix: "SIO_")
                .Build();

            if (!HasAzureServiceBusConnectionString())
                Skip = $"Skipping Azure Service Bus Test, no connection string configured.";
        }

        private bool HasAzureServiceBusConnectionString()
            => !string.IsNullOrWhiteSpace(_configuration["Azure:ServiceBus:ConnectionString"]);
    }
}
