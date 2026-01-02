using Microsoft.Extensions.Options;
using ResumeAnalyzer.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ResumeAnalyzer.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
   
        public async Task SendAsync(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_settings.SmtpHost, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _settings.Username,
                    _settings.Password
                ),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _settings.From,
                    _settings.DisplayName
                ),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
