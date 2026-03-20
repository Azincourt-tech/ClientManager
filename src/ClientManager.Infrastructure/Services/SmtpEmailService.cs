using System.Net;
using System.Net.Mail;
using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClientManager.Infrastructure.Services;

public class SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email, string name, byte[]? attachment = null, string? attachmentName = null)
    {
        var host = configuration["Smtp:Host"];
        var port = int.Parse(configuration["Smtp:Port"] ?? "587");
        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var fromEmail = configuration["Smtp:FromEmail"] ?? "no-reply@clientmanager.com";
        var fromName = configuration["Smtp:FromName"] ?? "ClientManager Team";

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username))
        {
            logger.LogWarning("SMTP not configured. Skipping email to {Email}", email);
            return;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Welcome to ClientManager!",
                Body = $"<h1>Hello {name},</h1><p>Welcome to our platform!</p>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(email, name));

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                var stream = new MemoryStream(attachment);
                mailMessage.Attachments.Add(new Attachment(stream, attachmentName, "application/pdf"));
                logger.LogInformation("Added attachment {AttachmentName} to email for {Email}", attachmentName, email);
            }

            await client.SendMailAsync(mailMessage);
            logger.LogInformation("Email sent successfully to {Email} via SMTP ({Host})", email, host);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to {Email} via SMTP", email);
        }
    }
}
