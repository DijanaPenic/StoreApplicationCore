using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class SetPasswordPatchApiModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}