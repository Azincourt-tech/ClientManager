using ClientManager.Infrastructure.CrossCutting.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace ClientManager.Infrastructure.CrossCutting.Ioc
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddRavenDb(this IServiceCollection servicesCollection, IConfiguration configuration)
        {
            servicesCollection.TryAddSingleton<IDocumentStore>(ctx =>
            {
                var url = configuration["RavenDbSettings:Url"] ?? "http://localhost:8080";
                var database = configuration["RavenDbSettings:Database"];
                var certPath = configuration["RavenDbSettings:CertificatePath"];
                var certPassword = configuration["RavenDbSettings:CertificatePassword"];

                var store = new DocumentStore
                {
                    Urls = [url],
                    Database = database
                };

                if (!string.IsNullOrEmpty(certPath))
                {
                    // O novo jeito: explícito, seguro e mais rápido
                    store.Certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);
                }
                // Remove a convenção automática que tenta mapear o ID do documento para a propriedade 'Id'
                // Isso permite que o domínio use Guid Id sem que o RavenDB tente forçar um string lá
                store.Conventions.FindIdentityProperty = member => member.Name == "NonExistentProperty";

                store.Conventions.FindCollectionName = type => type.Name;

                store.Initialize();

                return store;
            });

            return servicesCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddSingleton<ICustomerRepository, CustomerRepository>();
            servicesCollection.TryAddSingleton<IDocumentRepository, DocumentRepository>();

            return servicesCollection;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerService, CustomerService>();
            servicesCollection.TryAddScoped<IDocumentService, DocumentService>();

            return servicesCollection;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerApplication, CustomerApplication>();
            servicesCollection.TryAddScoped<IDocumentApplication, DocumentApplication>();

            return servicesCollection;
        }

        public static IServiceCollection AddValidators(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<IFileValidator, FileValidator>();
            servicesCollection.AddValidatorsFromAssemblyContaining<ClientManager.Application.Validators.CustomerValidator>();

            return servicesCollection;
        }
    }
}
