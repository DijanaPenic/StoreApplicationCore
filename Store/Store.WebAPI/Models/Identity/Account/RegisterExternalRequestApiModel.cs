using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class RegisterExternalRequestApiModel
    {
        public string Username { get; set; }

        public string AssociateEmail { get; set; }

        [Required]
        public bool AssociateExistingAccount { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; }
    }
}