using System;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateResponseApiModel
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string[] Roles { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public ExternalAuthStep ExternalAuthStep { get; set; }

        public VerificationStep VerificationStep { get; set; }
    }

    public enum ExternalAuthStep
    {
        None = 0,
        FoundExistingExternalLogin = 1,
        AddedNewExternalLogin = 2,
        PendingExternalLoginCreation = 3,
        UserNotFound = 4,
        UserNotAllowed = 5,                 
        UserEmailNotConfirmed = 6          
    }

    public enum VerificationStep
    {
        None = 0,
        TwoFactor = 1,
        Email = 2,
        MobilePhone = 3,
    }
}