using ClientManager.Infrastructure.CrossCutting.HealthChecks;
using ClientManager.Infrastructure.CrossCutting.Security;
using ClientManager.Infrastructure.CrossCutting.Settings;
using ClientManager.Infrastructure.CrossCutting.Validators;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Infrastructure.Services;
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
                var certBase64 = configuration["RavenDbSettings:CertificateBase64"];

                var store = new DocumentStore
                {
                    Urls = [url],
                    Database = database
                };

                if (!string.IsNullOrEmpty(certPath) && File.Exists(certPath))
                {
                    store.Certificate = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword);
                }
                else if (!string.IsNullOrEmpty(certBase64))
                {
                    byte[] certBytes = Convert.FromBase64String(certBase64);
                    store.Certificate = X509CertificateLoader.LoadPkcs12(certBytes, certPassword);
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
                session.Advanced.UseOptimisticConcurrency = true;
                return session;
            });

            // Registrar o Health Check
            servicesCollection.AddHealthChecks()
                .AddCheck<RavenDbHealthCheck>("RavenDB");

            return servicesCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerRepository, CustomerRepository>();
            servicesCollection.TryAddScoped<IDocumentRepository, DocumentRepository>();
            servicesCollection.TryAddScoped<IUserRepository, UserRepository>();

            return servicesCollection;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerService, CustomerService>();
            servicesCollection.TryAddScoped<IDocumentService, DocumentService>();
            servicesCollection.TryAddScoped<IUserService, UserService>();

            return servicesCollection;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<ICustomerApplication, CustomerApplication>();
            servicesCollection.TryAddScoped<IDocumentApplication, DocumentApplication>();
            servicesCollection.TryAddScoped<IUserApplication, UserApplication>();
            servicesCollection.TryAddScoped<IAuthApplication, AuthApplication>();

            return servicesCollection;
        }

        public static IServiceCollection AddValidators(this IServiceCollection servicesCollection)
        {
            servicesCollection.TryAddScoped<IFileValidator, FileValidator>();
            servicesCollection.AddValidatorsFromAssemblyContaining<ClientManager.Application.Validators.CustomerValidator>();

            return servicesCollection;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection servicesCollection, IConfiguration configuration)
        {
            servicesCollection.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
                throw new InvalidOperationException("JwtSettings configuration is missing or invalid.");

            servicesCollection.TryAddScoped<ITokenService, TokenService>();

            var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret);

            servicesCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

            servicesCollection.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("AdminOrManager", policy => policy.RequireRole("Admin", "Manager"));
                options.AddPolicy("AllRoles", policy => policy.RequireRole("Admin", "Manager", "Viewer"));
            });

            return servicesCollection;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection servicesCollection, IConfiguration configuration)
        {
            var useSmtp = configuration.GetSection("Smtp").Exists();
            if (useSmtp)
            {
                servicesCollection.TryAddScoped<IEmailService, SmtpEmailService>();
            }
            else
            {
                servicesCollection.TryAddScoped<IEmailService, SendGridEmailService>();
            }

            servicesCollection.TryAddScoped<IPdfGenerator, QuestPdfGenerator>();

            return servicesCollection;
        }
    }
}
