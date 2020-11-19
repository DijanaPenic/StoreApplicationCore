using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Identity
{
    public class SetPasswordPostApiModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}