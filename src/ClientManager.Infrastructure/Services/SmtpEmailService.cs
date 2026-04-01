using System.Globalization;
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
        var port = int.Parse(configuration["Smtp:Port"] ?? "587", CultureInfo.InvariantCulture);
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

    public async Task SendWelcomeEmailToUserAsync(string email, string username)
    {
        var host = configuration["Smtp:Host"];
        var port = int.Parse(configuration["Smtp:Port"] ?? "587", CultureInfo.InvariantCulture);
        var username2 = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        var fromEmail = configuration["Smtp:FromEmail"] ?? "no-reply@clientmanager.com";
        var fromName = configuration["Smtp:FromName"] ?? "ClientManager Team";

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username2))
        {
            logger.LogWarning("SMTP not configured. Skipping welcome email to {Email}", email);
            return;
        }

        try
        {
            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username2, password),
                EnableSsl = true
            };

            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff;'>
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                        <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>Welcome to ClientManager!</h1>
                    </div>
                    <div style='padding: 30px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 10px 10px;'>
                        <p style='color: #333333; font-size: 16px; line-height: 1.6;'>Hello <strong>{username}</strong>,</p>
                        <p style='color: #333333; font-size: 16px; line-height: 1.6;'>Congratulations! Your registration at <strong>ClientManager</strong> has been completed successfully.</p>
                        <p style='color: #333333; font-size: 16px; line-height: 1.6;'>You now have full access to our client and document management platform.</p>
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #667eea;'>
                            <p style='margin: 0 0 10px 0; color: #333333; font-weight: bold; font-size: 16px;'>Next steps:</p>
                            <ul style='color: #555555; font-size: 14px; line-height: 1.8; padding-left: 20px; margin: 0;'>
                                <li>Access your account and complete your profile</li>
                                <li>Start registering your clients</li>
                                <li>Upload and organize your documents</li>
                                <li>Explore all available features</li>
                            </ul>
                        </div>
                        <p style='color: #333333; font-size: 16px; line-height: 1.6;'>If you have any questions, our team is ready to help.</p>
                        <p style='color: #333333; font-size: 16px; line-height: 1.6;'>Best regards,<br><strong>ClientManager Team</strong></p>
                    </div>
                    <div style='text-align: center; padding: 20px; background-color: #f8f9fa; border-radius: 0 0 10px 10px;'>
                        <p style='color: #999999; font-size: 12px; margin: 0;'>© {DateTime.UtcNow.Year} ClientManager. All rights reserved.</p>
                    </div>
                </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Welcome to ClientManager!",
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(email, username));

            await client.SendMailAsync(mailMessage);
            logger.LogInformation("Welcome email sent successfully to {Email} via SMTP ({Host})", email, host);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending welcome email to {Email} via SMTP", email);
        }
    }
}
