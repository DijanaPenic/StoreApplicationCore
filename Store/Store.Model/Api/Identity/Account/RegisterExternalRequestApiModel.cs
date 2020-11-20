using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class RegisterExternalRequestApiModel
    {
        public string Username { get; set; }

        public string AssociateEmail { get; set; }

        [Required]
        public bool AssociateExistingAccount { get; set; }

        [Required]
        public string ClientId { get; set; }
    }
}