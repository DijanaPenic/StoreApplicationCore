using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class UserProfilePatchApiModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public string LastName { get; set; }
    }
}