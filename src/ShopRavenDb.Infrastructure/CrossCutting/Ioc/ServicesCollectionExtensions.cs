using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentValidation;
using ShopRavenDb.Domain.Core.Interfaces.Validators;
using ShopRavenDb.Infrastructure.CrossCutting.Validators;

namespace ShopRavenDb.Infrastructure.CrossCutting.Ioc
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddRavenDb(this IServiceCollection servicesCollection, IConfiguration configuration)
        {
            servicesCollection.TryAddSingleton<IDocumentStore>(ctx =>
            {
                var store = new DocumentStore
                {
                    Urls = new string[] { configuration["RavenDbSettings:Url"] ?? "http://localhost:8080" },
                    Database = configuration["RavenDbSettings:Database"] ?? "Shop"
                };

                store.Conventions.FindCollectionName = type => type.Name; // Simpler collection names
                
                store.Initialize();

                return store;
            });

            return servicesCollection;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection servicesCollection)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DtoToModelMappingCustomer());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            servicesCollection.AddSingleton(mapper);

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

        public static IServiceCollection AddRepositories(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddSingleton<ICustomerRepository, CustomerRepository>();
            servicesCollection.TryAddSingleton<IDocumentRepository, DocumentRepository>();

            return servicesCollection;
        }

        public static IServiceCollection AddValidators(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<IEmailValidator, EmailValidator>();
            servicesCollection.TryAddScoped<ICpfValidator, CpfValidator>();
            servicesCollection.TryAddScoped<ICnpjValidator, CnpjValidator>();
            servicesCollection.TryAddScoped<IFileValidator, FileValidator>();
            servicesCollection.AddValidatorsFromAssemblyContaining<ShopRavenDb.Application.Validators.CustomerDtoValidator>();

            return servicesCollection;
        }
    }
}