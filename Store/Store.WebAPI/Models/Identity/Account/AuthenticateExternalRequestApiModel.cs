using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class AuthenticateExternalRequestApiModel
    {
        [Required]
        public string ConfirmationUrl { get; set; }
    }
}