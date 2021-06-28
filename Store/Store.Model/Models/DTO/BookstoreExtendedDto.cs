using System;
using System.Collections.Generic;

namespace Store.Models
{
    public class BookstoreExtendedDto
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookDto> Books { get; set; }

        public int BooksCount { get; set; }
    }

    public class BookDto
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid BookstoreId { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }
}