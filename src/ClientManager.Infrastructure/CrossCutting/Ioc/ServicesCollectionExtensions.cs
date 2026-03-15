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
                    store.Certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);
                }

                store.Conventions.FindIdentityProperty = member => member.Name == "NonExistentProperty";
                store.Conventions.FindCollectionName = type => type.Name;

                store.Initialize();

                return store;
            });

            // Registrar a Sessão como Scoped (Unit of Work)
            servicesCollection.AddScoped(ctx =>
            {
                var store = ctx.GetRequiredService<IDocumentStore>();
                var session = store.OpenAsyncSession();
                session.Advanced.UseOptimisticConcurrency = true; // Ativa proteção contra Lost Updates
                return session;
            });

            return servicesCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerRepository, CustomerRepository>();
            servicesCollection.TryAddScoped<IDocumentRepository, DocumentRepository>();

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
