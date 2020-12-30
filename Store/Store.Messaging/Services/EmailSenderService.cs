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
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSenderAuthOptions _emailConfig;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailSenderService(IOptions<EmailSenderAuthOptions> options, IRazorViewToStringRenderer razorViewToStringRenderer, IEmailTemplateService emailTemplateService)
        {
            _emailConfig = options.Value;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendConfirmAccountEmailAsync(Guid clientId, string email, string url)
        {
            ConfirmAccountEmailViewModel confirmAccountModel = new ConfirmAccountEmailViewModel(url);
            string template = await GetHtmlEmailContentAsync(clientId, confirmAccountModel, EmailTemplateType.ConfirmAccount);

            await SendEmailAsync
            (
                email,
                $"Welcome to Store Application! Confirm Your Email",
                template
            );
        }

        public async Task SendConfirmExternalAccountEmailAsync(Guid clientId, string email, string url, string providerDisplayName)
        {
            ConfirmExternalAccountEmailViewModel confirmExternalAccountModel = new ConfirmExternalAccountEmailViewModel(url, providerDisplayName);
            string template = await GetHtmlEmailContentAsync(clientId, confirmExternalAccountModel, EmailTemplateType.ConfirmExternalAccount);

            await SendEmailAsync
            (
                email,
                $"Confirm {providerDisplayName} External Login - Store Application",
                template
            );
        }

        public async Task SendResetPasswordEmailAsync(Guid clientId, string email, string url, string userName)
        {
            ResetPasswordEmailViewModel resetPasswordModel = new ResetPasswordEmailViewModel(url, userName);
            string template = await GetHtmlEmailContentAsync(clientId, resetPasswordModel, EmailTemplateType.ResetPassword);

            await SendEmailAsync
            (
                 email,
                 $"Password Recovery - Store Application",
                 template
            );
        }

        public async Task SendChangePasswordEmailAsync(Guid clientId, string email, string userName, string newPassword)
        {
            ChangePasswordEmailViewModel changePasswordModel = new ChangePasswordEmailViewModel(userName, newPassword);
            string template = await GetHtmlEmailContentAsync(clientId, changePasswordModel, EmailTemplateType.ChangePassword);

            await SendEmailAsync
            (
                email,
                $"Password Changed - Store Application",
                template
            );
        }

        private string GetLocalEmailViewPath(EmailTemplateType type)
        {
            switch(type)
            {
                case EmailTemplateType.ConfirmAccount:
                    return EmailViewPath.ConfirmAccount;

                case EmailTemplateType.ResetPassword:
                    return EmailViewPath.ResetPassword;

                case EmailTemplateType.ConfirmExternalAccount:
                    return EmailViewPath.ConfirmExternalAccount;

                case EmailTemplateType.ChangePassword:
                    return EmailViewPath.ChangePassword;
            }

            return string.Empty;
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
                return await _razorViewToStringRenderer.RenderViewToStringAsync(GetLocalEmailViewPath(emailTemplate), emailModel);
            }
        }

        private static string ResolveEmailTemplatePlaceholders<T>(string template, T model)
        {
            StringBuilder sb = new StringBuilder(template);

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                sb.Replace($"{{{propertyInfo.Name}}}", propertyInfo.GetValue(model).ToString());
            }

            return sb.ToString();
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