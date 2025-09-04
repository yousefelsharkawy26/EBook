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

    
    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var fromAddress = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        var toAddress = new MailAddress(toEmail);

        
        var mailMessage = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true 
        };

        
        using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
        {
            smtpClient.EnableSsl = true; 
            smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
