using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Identity
{
    public class RolePostApiModel
    {
        [Required]
        public string Name { get; set; }

        public bool Stackable { get; set; }
    }
}