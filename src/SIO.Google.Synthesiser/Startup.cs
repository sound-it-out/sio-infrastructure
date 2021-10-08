using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIO.Infrastructure.Azure.ServiceBus.Extensions;
using SIO.Infrastructure.Azure.Storage.Extensions;
using SIO.Infrastructure.EntityFrameworkCore.SqlServer.Extensions;
using SIO.Infrastructure.Extensions;
using SIO.Infrastructure.Serialization.Json.Extensions;
using SIO.Infrastructure.Serialization.MessagePack.Extensions;

[assembly: FunctionsStartup(typeof(SIO.Google.Synthesiser.Startup))]

namespace SIO.Google.Synthesiser
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            builder.Services.AddSIOInfrastructure()
                .AddEvents()
                .AddAzureStorage(o => o.ConnectionString = config.GetConnectionString("AzureBlobStorage"))
                .AddAzureServiceBus(o => { })
                .AddJsonSerializers()
                .AddEntityFrameworkCoreSqlServer(options =>
                {
                });
        }
    }
}
