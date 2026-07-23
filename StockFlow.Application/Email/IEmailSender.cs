namespace StockFlow.Application.Email;

public interface IEmailSender
{
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        byte[]? attachment = null,
        string? attachmentFileName = null,
        string attachmentContentType = "application/octet-stream");
}
