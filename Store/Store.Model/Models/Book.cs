using System;

using Store.Model.Common.Models;

namespace Store.Models
{
    public class Book : IBook
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid BookstoreId { get; set; }

        public IBookstore Bookstore { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }
}