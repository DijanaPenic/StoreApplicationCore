using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class PasswordRecoveryPostApiModel : GoogleReCaptchaModelApiBase
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
    }
}