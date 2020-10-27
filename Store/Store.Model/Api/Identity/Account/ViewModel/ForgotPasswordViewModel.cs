using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}