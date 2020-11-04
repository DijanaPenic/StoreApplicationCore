using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class UserProfileGetModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("external_logins")]
        public IList<string> ExternalLogins { get; set; }

        [JsonPropertyName("two_factor_enabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonPropertyName("has_authenticator")]
        public bool HasAuthenticator { get; set; }

        [JsonPropertyName("two_factor_client_remembered")]
        public bool TwoFactorClientRemembered { get; set; }

        [JsonPropertyName("recovery_codes_left")]
        public int RecoveryCodesLeft { get; set; }
    }
}