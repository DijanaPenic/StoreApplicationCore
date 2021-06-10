using System;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models
{
    public interface IBook : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        Guid BookstoreId { get; set; }

        IBookstore Bookstore { get; set; }

        string Name { get; set; }

        string Author { get; set; }
    }
}