using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Models
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; }

        public IUserRefreshToken RefreshToken { get; set; }
    }
}