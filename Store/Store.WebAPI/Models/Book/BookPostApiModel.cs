using System;
using System.ComponentModel.DataAnnotations;

namespace Store.WebAPI.Models.Book
{
    public class BookPostApiModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Author { get; set; }

        [Required]
        public Guid BookstoreId { get; set; }
    }
}