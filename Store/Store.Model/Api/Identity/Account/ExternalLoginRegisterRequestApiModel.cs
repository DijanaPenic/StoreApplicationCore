using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class ExternalLoginRegisterRequestApiModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("external_login_email")]
        public string ExternalLoginEmail { get; set; }

        [JsonPropertyName("associate_email")]
        public string AssociateEmail { get; set; }

        [Required]
        [JsonPropertyName("associate_existing_account")]
        public bool AssociateExistingAccount { get; set; }

        [Required]
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
    }
}