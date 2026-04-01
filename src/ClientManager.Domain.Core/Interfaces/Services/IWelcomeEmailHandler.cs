namespace ClientManager.Domain.Core.Interfaces.Services;

/// <summary>
/// Synchronous handler for welcome email workflow (PDF generation + email send).
/// </summary>
public interface IWelcomeEmailHandler
{
    Task HandleCustomerCreatedAsync(Guid customerId, string name, string email);
}
