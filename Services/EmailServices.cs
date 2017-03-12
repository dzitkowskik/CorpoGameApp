using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using CorpoGameApp.Properties;
using Microsoft.Extensions.Options;
using SendGrid;

namespace CorpoGameApp.Services
{
    public interface IEmailServices
    {
        void SendTestEmail();
    }

    public class EmailServices : IEmailServices
    {
        private readonly NetworkCredential _credentials;
        private readonly MailAddress _from;

        public EmailServices(IOptions<ApplicationSettings> settings)
        {
            _from = new MailAddress(
                settings.Value.SendGridFromEmail,
                settings.Value.SendGridFromName);
            _credentials = new NetworkCredential(
                settings.Value.SendGridUsername, 
                settings.Value.SendGridPassword);
        }

        public async void SendEmail(string subject, string body, IEnumerable<string> recipients)
        {
            SendGridMessage myMessage = new SendGridMessage
            {
                From = _from,
                Subject = subject,
                Text = body
            };
            foreach(var recipient in recipients)
                myMessage.AddTo(recipient);
            var transportWeb = new Web(_credentials);
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}