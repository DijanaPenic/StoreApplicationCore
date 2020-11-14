using System;
using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class AuthenticateResponseApiModel
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("roles")]
        public string[] Roles { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("requires_two_factor")]
        public bool RequiresTwoFactor { get; set; }

        [JsonPropertyName("external_login_status")]
        public ExternalLoginStatus ExternalLoginStatus { get; set; }
    }

    public enum ExternalLoginStatus
    {
        None = 0,
        ExistingExternalLoginSuccess = 1,
        NewExternalLoginAddedSuccess = 2,
        PendingEmailConfirmation = 3,
    }
}