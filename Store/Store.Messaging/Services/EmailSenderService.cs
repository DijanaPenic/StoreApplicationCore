using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Store.Messaging.Options;
using Store.Messaging.Services.Common;
using Store.Messaging.Templates.Views.Email;
using Store.Messaging.Templates.Models.Email;
using Store.Messaging.Templates.Services.Email;

namespace Store.Messaging.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSenderAuthOptions _emailConfig;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public EmailSenderService(IOptions<EmailSenderAuthOptions> options, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _emailConfig = options.Value;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        public async Task SendConfirmAccountEmailAsync(string email, string url)
        {
            ConfirmAccountEmailViewModel confirmAccountModel = new ConfirmAccountEmailViewModel(url);

            await SendEmailAsync
            (
                email,
                $"Welcome to Store Application! Confirm Your Email",
                await _razorViewToStringRenderer.RenderViewToStringAsync(EmailViewPath.ConfirmAccount, confirmAccountModel)
            );
        }

        public async Task SendConfirmExternalAccountEmailAsync(string email, string url, string providerDisplayName)
        {
            ConfirmExternalAccountEmailViewModel confirmExternalAccountModel = new ConfirmExternalAccountEmailViewModel(url, providerDisplayName);

            await SendEmailAsync
            (
                email,
                $"Confirm {providerDisplayName} External Login - Store Application",
                await _razorViewToStringRenderer.RenderViewToStringAsync(EmailViewPath.ConfirmExternalAccount, confirmExternalAccountModel)
            );
        }

        public async Task SendResetPasswordEmailAsync(string email, string url, string userName)
        {
            ResetPasswordEmailViewModel resetPasswordModel = new ResetPasswordEmailViewModel(url, userName);

            await SendEmailAsync
            (
                email,
                $"Password Recovery - Store Application",
                await _razorViewToStringRenderer.RenderViewToStringAsync(EmailViewPath.ResetPassword, resetPasswordModel)
            );
        }

        public async Task SendChangePasswordEmailAsync(string email, string userName, string newPassword)
        {
            ChangePasswordEmailViewModel changePasswordModel = new ChangePasswordEmailViewModel(userName, newPassword);

            await SendEmailAsync
            (
                email,
                $"Password Changed - Store Application",
                await _razorViewToStringRenderer.RenderViewToStringAsync(EmailViewPath.ChangePassword, changePasswordModel)
            );
        }

        private Task SendEmailAsync(string email, string subject, string message)
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