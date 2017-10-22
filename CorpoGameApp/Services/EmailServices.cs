using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CorpoGameApp.Properties;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CorpoGameApp.Services
{

    public class EmailServices : IEmailServices
    {
        private readonly NetworkCredential _credentials;
        private readonly EmailAddress _from;

        private readonly string _sendGridApiKey;

        public EmailServices(IOptions<ApplicationSettings> settings)
        {
            _from = new EmailAddress(
                settings.Value.SendGridFromEmail,
                settings.Value.SendGridFromName);
            _credentials = new NetworkCredential(
                settings.Value.SendGridUsername, 
                settings.Value.SendGridPassword);
            _sendGridApiKey = settings.Value.SendGridApiKey;
        }

        public async Task<Response> SendEmail(string subject, string body, IEnumerable<string> recipients)
        {
            SendGridMessage myMessage = new SendGridMessage
            {
                From = _from,
                Subject = subject,
                PlainTextContent  = body
            };
            foreach(var recipient in recipients)
                myMessage.AddTo(recipient);

            var client = new SendGridClient(_sendGridApiKey);

            return await client.SendEmailAsync(myMessage);
        }
    }
}