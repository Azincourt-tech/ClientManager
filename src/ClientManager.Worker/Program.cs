using ClientManager.Infrastructure.CrossCutting.Ioc;
using ClientManager.Infrastructure.Messaging.DependencyInjection;
using ClientManager.Worker;
using ClientManager.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// Infrastructure
builder.Services.AddRavenDb(builder.Configuration);
builder.Services.AddRepositories();

// Domain Services
builder.Services.AddDomainServices();

// Infrastructure Services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Messaging
builder.Services.AddMessaging();

// Consumers (Scoped as they depend on Scoped services)
builder.Services.AddScoped<DocumentUploadedConsumer>();
builder.Services.AddScoped<CustomerCreatedConsumer>();

// Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
