using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Core;

namespace Store.Model.Common.Models
{
    public interface IBookstore : IBaseEntity, IChangable
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Location { get; set; }

        ICollection<IBook> Books { get; set; }
    }
}