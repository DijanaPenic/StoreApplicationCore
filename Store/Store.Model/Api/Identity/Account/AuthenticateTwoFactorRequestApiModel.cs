using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateTwoFactorRequestApiModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("use_recovery_code")]
        public bool UseRecoveryCode { get; set; }

        [Required]
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
    }
}