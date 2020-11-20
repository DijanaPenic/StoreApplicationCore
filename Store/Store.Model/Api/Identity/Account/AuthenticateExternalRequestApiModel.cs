using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateExternalRequestApiModel
    {
        [Required]
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}