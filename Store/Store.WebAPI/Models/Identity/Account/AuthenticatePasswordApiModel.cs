using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticatePasswordApiModel
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