using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClientManager.Infrastructure.Services;

/// <summary>
/// Handles the synchronous welcome email workflow:
/// 1. Generates a Welcome Kit PDF
/// 2. Sends a welcome email with the PDF attached
/// </summary>
public class WelcomeEmailHandler(
    IEmailService emailService,
    IPdfGenerator pdfGenerator,
    ILogger<WelcomeEmailHandler> logger) : IWelcomeEmailHandler
{
    public async Task HandleCustomerCreatedAsync(Guid customerId, string name, string email)
    {
        logger.LogInformation("Processing welcome email for customer {CustomerId} ({Name}, {Email})",
            customerId, name, email);

        try
        {
            // 1. Generate Welcome Kit PDF
            var pdfBytes = await pdfGenerator.GenerateWelcomeKitAsync(customerId, name);
            var attachmentName = $"WelcomeKit_{name.Replace(" ", "_")}.pdf";
            logger.LogInformation("Welcome kit PDF generated ({Size} bytes) for customer {CustomerId}",
                pdfBytes.Length, customerId);

            // 2. Send Welcome Email with PDF attachment
            await emailService.SendWelcomeEmailAsync(email, name, pdfBytes, attachmentName);

            logger.LogInformation("Welcome email flow completed for customer {CustomerId}", customerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing welcome email for customer {CustomerId}", customerId);
            // Don't rethrow — email failure shouldn't break the customer creation response
        }
    }
}
