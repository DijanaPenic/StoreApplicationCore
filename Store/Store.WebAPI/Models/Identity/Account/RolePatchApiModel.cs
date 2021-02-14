using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class RolePatchApiModel
    {
        [Required]
        public string Name { get; set; }

        public bool Stackable { get; set; }
    }
}