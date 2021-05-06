using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SIO.Google.Synthesiser.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleConfiguration(this IServiceCollection source, IConfiguration configuration)
        {
            source.Configure<GoogleCredentialOptions>(options =>
            {
                options.Type = configuration.GetValue<string>("Google__Credentials__type");
                options.ProjectId = configuration.GetValue<string>("Google__Credentials__project_id");
                options.PrivateKeyId = configuration.GetValue<string>("Google__Credentials__private_key_id");
                options.PrivateKey = configuration.GetValue<string>("Google__Credentials__private_key");
                options.ClientEmail = configuration.GetValue<string>("Google__Credentials__client_email");
                options.ClientId = configuration.GetValue<string>("Google__Credentials__client_id");
                options.AuthUri = configuration.GetValue<string>("Google__Credentials__auth_uri");
                options.TokenUri = configuration.GetValue<string>("Google__Credentials__token_uri");
                options.AuthProviderX509CertUrl = configuration.GetValue<string>("Google__Credentials__auth_provider_x509_cert_url");
                options.ClientX509CertUrl = configuration.GetValue<string>("Google__Credentials__client_x509_cert_url");
            });

            return source;
        }
    }
}
