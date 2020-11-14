using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateExternalLoginRequestApiModel
    {
        [Required]
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [Required]
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [Required]
        [JsonPropertyName("provider")]
        public string Provider { get; set; }
    }
}