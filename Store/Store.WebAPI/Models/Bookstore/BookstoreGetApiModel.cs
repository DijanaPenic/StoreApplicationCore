using System.Collections.Generic;

using Store.WebAPI.Models.Book;

namespace Store.WebAPI.Models.Bookstore
{
    public class BookstoreGetApiModel : ApiModelBase
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookGetApiModel> Books { get; set; }

        public int BooksCount { get; set; }
    }
}