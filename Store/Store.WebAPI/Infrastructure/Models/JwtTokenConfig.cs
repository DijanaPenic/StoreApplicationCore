﻿namespace Store.WebAPI.Infrastructure.Models
{
    public class JwtTokenConfig
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}