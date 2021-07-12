using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;

using Store.Common.Enums;
using Store.Messaging.Options;
using Store.Messaging.Services.Common;
using Store.Messaging.Templates.Views.Email;
using Store.Messaging.Templates.Models.Email;
using Store.Messaging.Templates.Services.Email;
using Store.Service.Common.Services;

namespace Store.Messaging.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridAuthOptions _config;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly SendGridClient _client;

        public EmailService(IOptions<SendGridAuthOptions> options, IRazorViewToStringRenderer razorViewToStringRenderer, IEmailTemplateService emailTemplateService)
        {
            _config = options.Value;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _emailTemplateService = emailTemplateService;
            _client = new SendGridClient(_config.ApiKey);
        }

        public async Task SendConfirmAccountAsync(Guid clientId, string email, string url)
        {
            ConfirmAccountViewModel confirmAccountModel = new(url);
            string template = await GetHtmlEmailContentAsync(clientId, confirmAccountModel, EmailTemplateType.ConfirmAccount);

            await SendEmailAsync
            (
                email,
                "Welcome to Store Application! Confirm Your Email",
                template
            );
        }

        public async Task SendConfirmEmailAsync(Guid clientId, string email, string url, string userName)
        {
            ConfirmEmailViewModel confirmEmailModel = new(userName, url);
            string template = await GetHtmlEmailContentAsync(clientId, confirmEmailModel, EmailTemplateType.ConfirmEmail);

            await SendEmailAsync
            (
                email,
                "Confirm Your Email - Store Application",
                template
            );
        }

        public async Task SendConfirmExternalAccountAsync(Guid clientId, string email, string url, string providerDisplayName)
        {
            ConfirmExternalAccountViewModel confirmExternalAccountModel = new(url, providerDisplayName);
            string template = await GetHtmlEmailContentAsync(clientId, confirmExternalAccountModel, EmailTemplateType.ConfirmExternalAccount);

            await SendEmailAsync
            (
                email,
                $"Confirm {providerDisplayName} External Login - Store Application",
                template
            );
        }

        public async Task SendResetPasswordAsync(Guid clientId, string email, string url, string userName)
        {
            ResetPasswordViewModel resetPasswordModel = new(url, userName);
            string template = await GetHtmlEmailContentAsync(clientId, resetPasswordModel, EmailTemplateType.ResetPassword);

            await SendEmailAsync
            (
                 email,
                 "Password Recovery - Store Application",
                 template
            );
        }

        public async Task SendChangePasswordEmailAsync(Guid clientId, string email, string userName, string newPassword)
        {
            ChangePasswordViewModel changePasswordModel = new(userName, newPassword);
            string template = await GetHtmlEmailContentAsync(clientId, changePasswordModel, EmailTemplateType.ChangePassword);

            await SendEmailAsync
            (
                email,
                "Password Changed - Store Application",
                template
            );
        }

        private async Task<string> GetHtmlEmailContentAsync<T>(Guid clientId, T emailModel, EmailTemplateType emailTemplate)
        {
            Stream templateStream = await _emailTemplateService.FindEmailTemplateByClientIdAsync(clientId, emailTemplate);

            // Get custom mail template (from storage - local or cloud)
            if (templateStream != null)
            {
                using StreamReader streamReader = new StreamReader(templateStream);

                return ResolveEmailTemplatePlaceholders(streamReader.ReadToEnd(), emailModel);
            }

            // Get default mail template (razor views)
            else
            {
                return await _razorViewToStringRenderer.RenderViewToStringAsync(EmailViewPath.GetViewPath(emailTemplate), emailModel);
            }
        }

        private static string ResolveEmailTemplatePlaceholders<T>(string template, T model)
        {
            StringBuilder sb = new(template);

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                sb.Replace($"{{{propertyInfo.Name}}}", propertyInfo.GetValue(model).ToString());
            }

            return sb.ToString();
        }

        private Task SendEmailAsync(string email, string subject, string message)
        {
            SendGridMessage msg = new()
            {
                From = new EmailAddress(_config.FromEmail, "Store Email Server"), 
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking: https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return _client.SendEmailAsync(msg);
        }
    }
}