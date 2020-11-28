using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Book
{
    public class BookPatchApiModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
        public string Author { get; set; }

        [Required]
        public Guid BookstoreId { get; set; }
    }
}