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

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var settings = _configuration.GetSection("EmailSettings");

        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings["SenderEmail"]!, settings["SenderName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        using var smtpClient = new SmtpClient(settings["SmtpServer"], int.Parse(settings["Port"]!))
        {
            Credentials = new NetworkCredential(settings["SenderEmail"], settings["Password"]),
            EnableSsl = true
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}
