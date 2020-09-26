using System.ComponentModel.DataAnnotations;

namespace Store.Models.Api.Bookstore
{
    public class BookstorePatchApiModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }
    }
}