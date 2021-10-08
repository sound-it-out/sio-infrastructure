using System;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Events;
using SIO.Infrastructure.Domain;
using Microsoft.Extensions.Options;

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

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Func<IServiceProvider, Action<TOptions>> configureOptions)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>(sp => new ConfigureNamedOptions<TOptions>(null, configureOptions(sp)));
            return services;
        }
    }
}
