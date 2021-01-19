using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class EmailConfirmationPostApiModel
    {
        [Required]
        public string ReturnUrl { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
    }
}