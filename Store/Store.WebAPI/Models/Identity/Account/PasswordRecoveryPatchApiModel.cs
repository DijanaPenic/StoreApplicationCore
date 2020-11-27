using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class PasswordRecoveryPatchApiModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        public string PasswordRecoveryToken { get; set; }
    }
}