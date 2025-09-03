using Digital_Library.Service.Interface;
using System.Net;
using System.Net.Mail;

namespace Digital_Library.Service.Implementation
{
    public class MailService : IMailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _from;

        public MailService(string host, int port, string username, string password, string from)
        {
            _from = from;
            _smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var message = new MailMessage(_from, to, subject, body)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(message);
        }
    }
}
