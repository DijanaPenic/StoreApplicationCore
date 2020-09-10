﻿using System;

using Store.Model.Common.Models.Core;

namespace Store.Models
{
    public class BookTDO : IPoco
    {
        public Guid Id { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public Guid BookstoreId { get; set; }

        public BookstoreDTO Bookstore { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }
    }
}