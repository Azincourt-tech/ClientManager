
using ClientManager.Api;
using ClientManager.Api.Middlewares;
using ClientManager.Infrastructure.Messaging.DependencyInjection;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnetcore/openapi
builder.Services.AddOpenApi();

builder.Services.AddRavenDb(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddValidators();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMessaging();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

var supportedCultures = new[] { "en-US", "pt-BR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/openapi/{documentName}.json")
           .WithTheme(ScalarTheme.DeepSpace)
           .HideClientButton()
           .DisableAgent()
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
           .HideDeveloperTools();
});

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
