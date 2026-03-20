using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ClientManager.Infrastructure.Services;

public class SendGridEmailService(IConfiguration configuration, ILogger<SendGridEmailService> logger) : IEmailService
{
    public async Task SendWelcomeEmailAsync(string email, string name, byte[]? attachment = null, string? attachmentName = null)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        var fromEmail = configuration["SendGrid:FromEmail"] ?? "no-reply@clientmanager.com";
        var fromName = configuration["SendGrid:FromName"] ?? "ClientManager Team";

        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_SENDGRID_API_KEY")
        {
            logger.LogWarning("SendGrid ApiKey not configured. Skipping email to {Email}", email);
            return;
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var subject = "Bem-vindo ao ClientManager!";
        var to = new EmailAddress(email, name);
        
        var plainTextContent = $"Olá {name}, bem-vindo ao ClientManager! Seu cadastro foi realizado com sucesso. Segue anexo seu Kit de Boas-vindas.";
        var htmlContent = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee;'>
                <h1 style='color: #2c3e50;'>Bem-vindo, {name}!</h1>
                <p>Estamos muito felizes em ter você como nosso cliente no <strong>ClientManager</strong>.</p>
                <p>Seu cadastro foi concluído e agora você pode começar a gerenciar seus documentos com facilidade.</p>
                <p><strong>Em anexo, você encontrará seu Kit de Boas-vindas em PDF.</strong></p>
                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Próximos passos:</strong></p>
                    <ul>
                        <li>Faça o upload dos seus documentos.</li>
                        <li>Aguarde a verificação automática.</li>
                        <li>Explore as funcionalidades da nossa plataforma.</li>
                    </ul>
                </div>
                <p>Se tiver qualquer dúvida, responda a este e-mail.</p>
                <p>Atenciosamente,<br>Equipe ClientManager</p>
            </div>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        if (attachment != null && !string.IsNullOrEmpty(attachmentName))
        {
            var base64Content = Convert.ToBase64String(attachment);
            msg.AddAttachment(attachmentName, base64Content, "application/pdf");
            logger.LogInformation("Added attachment {AttachmentName} to email for {Email}", attachmentName, email);
        }
        
        try
        {
            var response = await client.SendEmailAsync(msg);
            
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email sent successfully to {Email} via SendGrid", email);
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync();
                logger.LogError("Failed to send email to {Email}. Status: {Status}. Error: {Error}", 
                    email, response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while sending email to {Email} via SendGrid", email);
        }
    }
}
