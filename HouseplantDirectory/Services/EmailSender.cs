using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace HouseplantDirectory.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _fromAddress;
        private readonly string _fromName;
        private readonly string _userName;
        private readonly string _userPassword;
        private readonly string _host;
        private readonly int _port;
        public EmailSender(IConfiguration config)
        {
            _fromAddress = config.GetValue<string>("SMTP:FromAddress");
            _fromName = config.GetValue<string>("SMTP:FromName");
            _userName = config.GetValue<string>("SMTP:UserName");
            _userPassword = config.GetValue<string>("SMTP:UserPassword");
            _host = config.GetValue<string>("SMTP:Host");
            _port = config.GetValue<int>("SMTP:Port");
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient();
            client.Host = _host;
            client.Port = _port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_userName, _userPassword);
            using var message = new MailMessage
            (
                from: new MailAddress(_fromAddress, _fromName),
                to: new MailAddress(email)
            );
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;
            await client.SendMailAsync(message);
        }
    }
}
