using System;

using Store.Common.Enums;

namespace Store.Messaging.Templates.Views.Email
{
    public static class EmailViewPath
    {
        public const string ConfirmAccount = "/Views/Email/ConfirmAccount/ConfirmAccount.cshtml";

        public const string ConfirmExternalAccount = "/Views/Email/ConfirmAccount/ConfirmExternalAccount.cshtml";

        public const string ConfirmEmail = "/Views/Email/ConfirmAccount/ConfirmEmail.cshtml";

        public const string ResetPassword = "/Views/Email/Password/ResetPassword.cshtml";

        public const string ChangePassword = "/Views/Email/Password/ChangePassword.cshtml";

        public static string GetViewPath(EmailTemplateType type)
        {
            return type switch
            {
                EmailTemplateType.ConfirmAccount => ConfirmAccount,
                EmailTemplateType.ResetPassword => ResetPassword,
                EmailTemplateType.ConfirmExternalAccount => ConfirmExternalAccount,
                EmailTemplateType.ChangePassword => ChangePassword,
                EmailTemplateType.ConfirmEmail => ConfirmEmail,
                _ => throw new NotImplementedException("Invalid Email Template Type!")
            };
        }
    }
}