using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class RenewTokenRequestApiModel
    {
        public string RefreshToken { get; set; }

        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}