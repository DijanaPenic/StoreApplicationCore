using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class RenewTokenRequestApiModel
    {
        public string RefreshToken { get; set; }

        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}