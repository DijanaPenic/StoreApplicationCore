using System.Collections.Generic;

using Store.Models.Api.Book;

namespace Store.Models.Api.Bookstore
{
    public class BookstoreGetApiModel : ApiModelBase
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookGetApiModel> Books { get; set; }

        public int BooksCount { get; set; }
    }
}