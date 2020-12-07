using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Store.Service.Common.Services;
using Store.WebAPI.Infrastructure.Models;

namespace Store.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSenderAuthOptions _emailConfig; 
        
        public EmailSenderService(IOptions<EmailSenderAuthOptions> options)
        {
            _emailConfig = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SendGridClient client = new SendGridClient(_emailConfig.Key);

            SendGridMessage msg = new SendGridMessage()
            {
                From = new EmailAddress(_emailConfig.FromEmail, "Store Email Server"), 
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking: https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}