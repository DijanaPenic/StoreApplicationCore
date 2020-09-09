using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities
{
    [Table("Book")]
    public class BookEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        [ForeignKey("Bookstore")]
        public Guid BookstoreId { get; set; }

        public BookstoreEntity Bookstore { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
        public string Author { get; set; }
    }
}