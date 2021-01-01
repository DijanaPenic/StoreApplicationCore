using System;

using Store.Common.Enums;

namespace Store.Messaging.Templates.Views.Email
{
    public static class EmailViewPath
    {
        public const string ConfirmAccount = "/Views/Email/ConfirmAccount/ConfirmAccountEmail.cshtml";

        public const string ConfirmExternalAccount = "/Views/Email/ConfirmAccount/ConfirmExternalAccountEmail.cshtml";

        public const string ResetPassword = "/Views/Email/Password/ResetPasswordEmail.cshtml";

        public const string ChangePassword = "/Views/Email/Password/ChangePasswordEmail.cshtml";

        public static string GetViewPath(EmailTemplateType type)
        {
            return type switch
            {
                EmailTemplateType.ConfirmAccount => ConfirmAccount,
                EmailTemplateType.ResetPassword => ResetPassword,
                EmailTemplateType.ConfirmExternalAccount => ConfirmExternalAccount,
                EmailTemplateType.ChangePassword => ChangePassword,
                _ => throw new NotImplementedException("Invalid Email Template Type!")
            };
        }
    }
}