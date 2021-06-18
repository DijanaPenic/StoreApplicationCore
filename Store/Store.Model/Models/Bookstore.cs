using System;
using System.Collections.Generic;

using Store.Model.Common.Models;

namespace Store.Models
{
    public class Bookstore : IBookstore
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<IBook> Books { get; set; } 
    }
}