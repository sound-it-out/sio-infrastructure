using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SIO.Infrastructure.Testing.Abstractions;
using Xunit.Abstractions;

namespace SIO.Infrastructure.Testing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddXunitLogging(this IServiceCollection services, ITestOutputHelper testOutputHelper)
        {
            services.AddSingleton(testOutputHelper);
            services.AddTransient(typeof(ILogger<>), typeof(XunitLogger<>));
            return services;
        }
    }
}
