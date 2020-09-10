using System.Collections.Generic;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models
{
    public interface IBookstore : IPoco
    {
        string Name { get; set; }

        string Location { get; set; }

        ICollection<IBook> Books { get; set; }

        int BooksCount { get; set; }
    }
}