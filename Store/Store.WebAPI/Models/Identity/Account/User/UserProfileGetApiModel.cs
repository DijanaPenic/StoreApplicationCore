namespace Store.WebAPI.Models.Identity
{
    public class UserProfileGetApiModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public ExternalLoginGetApiModel[] ExternalLogins { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool HasAuthenticator { get; set; }

        public bool TwoFactorClientRemembered { get; set; }

        public int RecoveryCodesLeft { get; set; }
    }
}