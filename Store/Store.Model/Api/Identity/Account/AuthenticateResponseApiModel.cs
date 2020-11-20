using System;

namespace Store.Models.Api.Identity
{
    public class AuthenticateResponseApiModel
    {
        public Guid UserId { get; set; }

        public string[] Roles { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public bool RequiresTwoFactor { get; set; }

        public ExternalLoginStatus ExternalLoginStatus { get; set; }
    }

    public enum ExternalLoginStatus
    {
        None = 0,
        ExistingExternalLoginSuccess = 1,
        NewExternalLoginAddedSuccess = 2,
        PendingEmailConfirmation = 3,
        UserAccountNotFound = 4,
        UserNotAllowed = 5,
        EmailRequiresConfirmation = 6
    }
}