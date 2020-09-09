using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities
{
    [Table("Bookstore")]
    public class BookstoreEntity : IDBPoco
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Location cannot be longer than 100 characters.")]
        public string Location { get; set; }

        public ICollection<BookEntity> Books { get; set; }
    }
}