using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class RefreshTokenResponseApiModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}