using System;
using System.Collections.Generic;

namespace Store.Models
{
    public class BookstoreExtendedDTO
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookDTO> Books { get; set; } 

        public int BooksCount { get; set; }
    }

    public class BookDTO
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid BookstoreId { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }
}