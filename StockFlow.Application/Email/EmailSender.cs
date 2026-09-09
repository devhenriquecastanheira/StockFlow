using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace StockFlow.Application.Email;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        byte[]? attachment = null,
        string? attachmentFileName = null,
        string attachmentContentType = "application/octet-stream")
    {
        var settings = _configuration.GetSection("EmailSettings");
        var smtpServer = settings["SmtpServer"];
        var port = settings["Port"];
        var senderName = settings["SenderName"];
        var senderEmail = settings["SenderEmail"];
        var password = settings["Password"];

        if (string.IsNullOrWhiteSpace(smtpServer) ||
            string.IsNullOrWhiteSpace(port) ||
            string.IsNullOrWhiteSpace(senderEmail) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("As configurações de envio de email estão incompletas.");
        }

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        if (attachment is not null && attachment.Length > 0)
        {
            var attachmentStream = new MemoryStream(attachment);
            mailMessage.Attachments.Add(new Attachment(
                attachmentStream,
                attachmentFileName ?? "anexo",
                attachmentContentType));
        }

        using var smtpClient = new SmtpClient(smtpServer, int.Parse(port))
        {
            Credentials = new NetworkCredential(senderEmail, password),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
