using System.Net;
using System.Net.Mail;
using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClientManager.Infrastructure.Services;

public class ResendEmailService(IConfiguration configuration, ILogger<ResendEmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email, string name, byte[]? attachment = null, string? attachmentName = null)
    {
        var apiKey = configuration["Resend:ApiKey"];
        var fromEmail = configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
        var fromName = configuration["Resend:FromName"] ?? "ClientManager";

        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogWarning("Resend API key not configured. Skipping email to {Email}", email);
            return;
        }

        try
        {
            // Resend aceita SMTP com a API key como password
            using var client = new SmtpClient("smtp.resend.com", 465)
            {
                Credentials = new NetworkCredential("resend", apiKey),
                EnableSsl = true
            };

            var htmlContent = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee;'>
                    <h1 style='color: #2c3e50;'>Bem-vindo, {name}!</h1>
                    <p>Estamos muito felizes em ter voc como nosso cliente no <strong>ClientManager</strong>.</p>
                    <p>Seu cadastro foi concludo e agora voc pode come\u00e7ar a gerenciar seus documentos com facilidade.</p>
                    <p><strong>Em anexo, voc encontrar seu Kit de Boas-vindas em PDF.</strong></p>
                    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p style='margin: 0;'><strong>Prximos passos:</strong></p>
                        <ul>
                            <li>Faa o upload dos seus documentos.</li>
                            <li>Aguarde a verificao automtica.</li>
                            <li>Explore as funcionalidades da nossa plataforma.</li>
                        </ul>
                    </div>
                    <p>Se tiver qualquer dvida, responda a este e-mail.</p>
                    <p>Atenciosamente,<br>Equipe ClientManager</p>
                </div>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Bem-vindo ao ClientManager!",
                Body = htmlContent,
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
            logger.LogInformation("Email sent successfully to {Email} via Resend", email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to {Email} via Resend", email);
        }
    }
}
