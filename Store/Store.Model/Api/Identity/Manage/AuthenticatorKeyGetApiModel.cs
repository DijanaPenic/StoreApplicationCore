using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class AuthenticatorKeyGetApiModel
    {
        [JsonPropertyName("shared_key")]
        public string SharedKey { get; set; }

        [JsonPropertyName("authenticator_uri")]
        public string AuthenticatorUri { get; set; }
    }
}