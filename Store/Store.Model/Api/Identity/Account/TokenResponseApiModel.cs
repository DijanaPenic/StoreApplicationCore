using System;
using System.Text.Json.Serialization;

namespace Store.Models.Api.Identity
{
    public class TokenResponseApiModel
    {
        [JsonPropertyName("confirmation_token")]
        public string ConfirmationToken { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }
    }
}