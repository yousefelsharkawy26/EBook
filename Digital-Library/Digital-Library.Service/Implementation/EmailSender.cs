using Digital_Library.Core.Constant;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Digital_Library.Service.Implementation;
public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    // The constructor gets the settings via dependency injection
    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var fromAddress = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        var toAddress = new MailAddress(toEmail);

        // Create the email message
        var mailMessage = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true // This is crucial for sending HTML content
        };

        // Create the SmtpClient and configure it
        using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
        {
            smtpClient.EnableSsl = true; // Gmail requires SSL
            smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password);

            // Send the email asynchronously
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
