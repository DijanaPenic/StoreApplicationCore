using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class RegisterExternalRequestApiModel
    {
        public string UserName { get; set; }

        public string AssociateEmail { get; set; }

        [Required]
        public bool AssociateExistingAccount { get; set; }

        [Required]
        public string ConfirmationUrl { get; set; }       
            
        [Required]
        public string ClientId { get; set; }
    }
}