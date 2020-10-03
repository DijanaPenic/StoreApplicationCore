using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Store.Reporting.Models
{
    public class BookstoreReportPOCO
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public IEnumerable<BookReportPOCO> Books { get; set; }

        public int BooksCount { get; set; }
    }

    [DataObject]
    public class Bookstores
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<BookstoreReportPOCO> GetMockBookstores()
        {
            Books books = new Books();

            return new List<BookstoreReportPOCO>
            {
                new BookstoreReportPOCO
                {
                    Id = Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Name = "Bookstore Name #1",
                    Location = "Bookstore Location #1",
                    Books = books.GetMockBooksByBookstoreId(Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"))
                },
                new BookstoreReportPOCO
                {
                    Id = Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"),
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Name = "Bookstore Name #2",
                    Location = "Bookstore Location #2",
                    Books = books.GetMockBooksByBookstoreId(Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"))
                }
            };

        }
    }
}