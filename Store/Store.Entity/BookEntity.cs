using System;

namespace Store.Entities
{
    public class BookEntity : IDBBaseEntity, IDbChangeable
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid BookstoreId { get; set; }

        public BookstoreEntity Bookstore { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }
}