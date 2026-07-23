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

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(settings["SenderEmail"]!, settings["SenderName"]),
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

        using var smtpClient = new SmtpClient(settings["SmtpServer"], int.Parse(settings["Port"]!))
        {
            Credentials = new NetworkCredential(settings["SenderEmail"], settings["Password"]),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
