namespace ClientManager.Domain.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email, string name, byte[]? attachment = null, string? attachmentName = null);
}
