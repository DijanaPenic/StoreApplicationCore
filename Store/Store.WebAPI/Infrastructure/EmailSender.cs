using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

using Store.WebAPI.Infrastructure.Models;

namespace Store.WebAPI.Infrastructure
{
    public class EmailSender : IEmailSender
    {
        private readonly AuthMessageSenderOptions _options; 
        
        public EmailSender(AuthMessageSenderOptions options)
        {
            _options = options;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            SendGridClient client = new SendGridClient(_options.SendGridKey);

            SendGridMessage msg = new SendGridMessage()
            {
                From = new EmailAddress("penic.dijana@gmail.com", "Store Email Server"), // TODO - can only use verified email domain or email address
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

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}