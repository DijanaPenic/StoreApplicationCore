using System;
using System.Collections.Generic;

namespace Store.Entities
{
    public class BookstoreEntity : IDBBaseEntity, IDBChangable
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public ICollection<BookEntity> Books { get; set; }
    }
}