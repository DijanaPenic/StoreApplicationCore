using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateExternalRequestApiModel
    {
        [Required]
        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [Required]
        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }
    }
}