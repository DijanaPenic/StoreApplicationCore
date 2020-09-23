using System;

namespace Store.Models.Api.Book
{
    public class BookGetApiModel : BaseApiModel
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public BookstoreApiModel Bookstore { get; set; }
    }

    public class BookstoreApiModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }
    }
}