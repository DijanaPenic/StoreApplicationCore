using System;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateResponseApiModel
    {
        public Guid UserId { get; set; }

        public string[] Roles { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public bool RequiresTwoFactor { get; set; }

        public ExternalLoginStep ExternalLoginStep { get; set; }

        public VerificationStep VerificationStep { get; set; }
    }

    public enum ExternalLoginStep
    {
        None = 0,
        ExistingExternalLoginSuccess = 1,
        NewExternalLoginAddedSuccess = 2,
        PendingEmailConfirmation = 3,
        UserAccountNotFound = 4,
        UserNotAllowed = 5,                 
        EmailRequiresConfirmation = 6           // TODO - need to check the flow
    }

    public enum VerificationStep
    {
        None = 0,
        TwoFactor = 1,
        Email = 2,
        MobilePhone = 3,
    }
}