using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Bookstore
{
    public class BookstorePostApiModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Location cannot be longer than 50 characters.")]
        public string Location { get; set; }
    }
}