using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class EmailConfirmationPostApiModel
    {
        [Required]
        public string ReturnUrl { get; set; }
    }
}