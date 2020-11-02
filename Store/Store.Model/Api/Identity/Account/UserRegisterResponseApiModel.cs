using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class UserRegisterResponseApiModel
    {
        [JsonPropertyName("confirmation_token")]
        public string ConfirmationToken { get; set; }
    }
}