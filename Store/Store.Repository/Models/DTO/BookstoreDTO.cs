using System;
using System.Collections.Generic;

namespace Store.Repository.Models
{
    public class BookstoreDTO
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookDTO> Books { get; set; } 

        public int BooksCount { get; set; }
    }
}