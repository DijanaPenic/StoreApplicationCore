using System;

namespace Store.WebAPI.Models.Book
{
    public class BookGetApiModel : ApiModelBase
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