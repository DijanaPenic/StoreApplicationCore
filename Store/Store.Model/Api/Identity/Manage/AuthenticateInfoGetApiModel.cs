using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class AuthenticateInfoGetApiModel
    {
        [JsonPropertyName("is_authenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod { get; set; }

        [JsonPropertyName("display_set_password")]
        public bool DisplaySetPassword { get; set; }
    }
}