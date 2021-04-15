using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Domain;

namespace SIO.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ISIOInfrastructureBuilder AddSIOInfrastructure(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IAggregateFactory, AggregateFactory>();
            services.AddSingleton<IEventTypeCache, EventTypeCache>(); 

            return new SIOInfrastructureBuilder(services);
        }
    }
}
