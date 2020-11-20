using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateRequestApiModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}