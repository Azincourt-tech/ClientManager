using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClientManager.Infrastructure.Services;

public class SimulatedPdfGenerator(ILogger<SimulatedPdfGenerator> logger) : IPdfGenerator
{
    public async Task<byte[]> GenerateWelcomeKitAsync(Guid customerId, string name)
    {
        logger.LogInformation("Generating Welcome Kit PDF for customer: {Name} ({CustomerId})", name, customerId);

        // Simulate some CPU-bound PDF generation time
        await Task.Delay(2500);

        // Return a dummy byte array representing a PDF
        var dummyPdf = "This is a simulated PDF Welcome Kit for " + name;
        logger.LogInformation("Welcome Kit PDF generated successfully for {CustomerId}", customerId);
        
        return System.Text.Encoding.UTF8.GetBytes(dummyPdf);
    }
}
