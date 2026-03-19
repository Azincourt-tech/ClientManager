using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClientManager.Infrastructure.Services;

public class SimulatedEmailService(ILogger<SimulatedEmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email, string name)
    {
        logger.LogInformation("Sending welcome email to: {Name} ({Email})", name, email);
        
        // Simulating network delay
        await Task.Delay(1000);
        
        logger.LogInformation("Welcome email sent successfully to {Email}", email);
    }
}
