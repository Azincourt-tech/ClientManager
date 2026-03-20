using ClientManager.Domain.Core.Events;
using ClientManager.Domain.Core.Interfaces;
using ClientManager.Worker.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace ClientManager.Worker;

public class Worker(
    ILogger<Worker> logger,
    IMessageBus messageBus,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker starting and subscribing to queues...");

        await messageBus.SubscribeAsync<DocumentUploadedEvent>("document-uploaded", async (@event) =>
        {
            using var scope = scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<DocumentUploadedConsumer>();
            await consumer.HandleAsync(@event);
        });

        await messageBus.SubscribeAsync<CustomerCreatedEvent>("customer-created", async (@event) =>
        {
            using var scope = scopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<CustomerCreatedConsumer>();
            await consumer.HandleAsync(@event);
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
