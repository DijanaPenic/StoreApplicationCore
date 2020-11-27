using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class SetPasswordPatchApiModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}