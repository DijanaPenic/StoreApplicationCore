using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class AuthenticateTwoFactorRequestApiModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Code { get; set; }

        public bool UseRecoveryCode { get; set; }

        [Required]
        public string ClientId { get; set; }
    }
}