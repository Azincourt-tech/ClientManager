using ClientManager.Domain.Core.Events;
using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClientManager.Worker.Consumers;

public class CustomerCreatedConsumer(
    IEmailService emailService,
    IPdfGenerator pdfGenerator,
    ILogger<CustomerCreatedConsumer> logger)
{
    public async Task HandleAsync(CustomerCreatedEvent @event)
    {
        logger.LogInformation("Processing customer creation for: {Name} ({Email})", @event.Name, @event.Email);

        try
        {
            // 1. Send Welcome Email
            await emailService.SendWelcomeEmailAsync(@event.Email, @event.Name);

            // 2. Generate Welcome Kit PDF
            var pdfBytes = await pdfGenerator.GenerateWelcomeKitAsync(@event.CustomerId, @event.Name);
            logger.LogInformation("Welcome kit PDF generated ({Size} bytes) for {CustomerId}", pdfBytes.Length, @event.CustomerId);

            logger.LogInformation("Welcome flow completed for customer {CustomerId}", @event.CustomerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing welcome flow for customer {CustomerId}", @event.CustomerId);
            throw; // RabbitMQ will retry based on configuration
        }
    }
}
