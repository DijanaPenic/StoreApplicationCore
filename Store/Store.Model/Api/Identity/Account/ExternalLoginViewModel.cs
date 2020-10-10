using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}