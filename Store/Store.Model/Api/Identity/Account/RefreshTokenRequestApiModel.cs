﻿using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class RefreshTokenRequestApiModel
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [Required]
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
    }
}