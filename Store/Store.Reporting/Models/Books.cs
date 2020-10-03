using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using Store.Common.Helpers;

namespace Store.Reporting.Models
{
    public class BookReportPOCO
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public Guid BookstoreId { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }

    [DataObject]
    public class Books
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<BookReportPOCO> GetMockBooks()
        {
            return new List<BookReportPOCO>
            {
                new BookReportPOCO 
                { 
                    Id = GuidHelper.NewSequentialGuid(), 
                    Author = "Book Author #1", 
                    BookstoreId = Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #1"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #2",
                    BookstoreId = Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #2"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #3", 
                    BookstoreId = Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"),
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #3"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #4", 
                    BookstoreId = Guid.Parse("6d9f5ccf-c60a-450c-a724-dd39a1b76b28"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #4"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #5", 
                    BookstoreId = Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #5"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #6",
                    BookstoreId = Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #6"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #7", 
                    BookstoreId = Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #7"
                },
                new BookReportPOCO 
                {
                    Id = GuidHelper.NewSequentialGuid(),
                    Author = "Book Author #8", 
                    BookstoreId = Guid.Parse("bb245ec5-0521-47ca-ad1b-7702f08f27d1"), 
                    DateCreated = DateTime.Now, 
                    DateUpdated = DateTime.Now, 
                    Name = "Book Name #8"
                }
            };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IEnumerable<BookReportPOCO> GetMockBooksByBookstoreId(Guid bookstoreId)
        {
            return GetMockBooks().Where(b => b.BookstoreId == bookstoreId);
        }
    }
}