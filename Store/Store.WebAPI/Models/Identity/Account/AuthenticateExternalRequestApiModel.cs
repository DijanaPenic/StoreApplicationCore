using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateExternalRequestApiModel
    {
        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; }
    }
}