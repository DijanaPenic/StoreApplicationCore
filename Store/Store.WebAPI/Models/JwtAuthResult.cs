﻿using System.Collections.Generic;

namespace Store.WebAPI.Models
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public IList<string> Roles { get; set; }
    }
}