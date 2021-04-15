using System;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Infrastructure
{
    internal class SIOInfrastructureBuilder : ISIOInfrastructureBuilder
    {
        public IServiceCollection Services { get; }

        public SIOInfrastructureBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
