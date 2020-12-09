using System.Collections.Generic;

using Store.Service.Common.Services.Identity;

namespace Store.Services.Identity
{
    public class JwtAuthResult : IJwtAuthResult
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public IList<string> Roles { get; set; }
    }
}