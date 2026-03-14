using FluentValidation;
using Microsoft.Extensions.Configuration;
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

                // Remove a convenção automática que tenta mapear o ID do documento para a propriedade 'Id'
                // Isso permite que o domínio use Guid Id sem que o RavenDB tente forçar um string lá
                store.Conventions.FindIdentityProperty = member => member.Name == "NonExistentProperty";

                store.Conventions.FindCollectionName = type => type.Name;

                store.Initialize();

                return store;
            });

            return servicesCollection;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection servicesCollection)
        {
            servicesCollection.AddSingleton<IMapper>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new DtoToModelMappingCustomer());
                }, loggerFactory);

                return mappingConfig.CreateMapper();
            });

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