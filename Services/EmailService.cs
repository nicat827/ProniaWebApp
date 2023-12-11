using Pronia.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Pronia.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string receiver, string body, string subject, bool isHtml=true)
        {
            SmtpClient smtpClient = new SmtpClient(_config["ApplicationEmail:Host"], Convert.ToInt32(_config["ApplicationEmail:Port"]));
            smtpClient.EnableSsl = true;

            smtpClient.Credentials = new NetworkCredential(_config["ApplicationEmail:Email"], _config["ApplicationEmail:Password"]);

            MailAddress from = new MailAddress(_config["ApplicationEmail:Email"], "Pronia");
            MailAddress to = new MailAddress(receiver);

            MailMessage message = new MailMessage(from, to);

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            await smtpClient.SendMailAsync(message);

        }
    }
}
