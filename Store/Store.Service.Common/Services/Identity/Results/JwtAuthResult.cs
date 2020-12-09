using System.Collections.Generic;

namespace Store.Service.Common.Services.Identity
{
    public interface IJwtAuthResult
    {
        string AccessToken { get; set; }

        string RefreshToken { get; set; }

        IList<string> Roles { get; set; }
    }
}