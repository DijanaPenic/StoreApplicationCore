using System.Collections.Generic;

namespace Store.Models.Api
{
    public class PaginationMetaData
    {
        public int FirstItemOnPage { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool IsFirstPage { get; set; }

        public bool IsLastPage { get; set; }

        public int LastItemOnPage { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }
    }

    public class PaginationEntity<T>
    {
        public PaginationEntity()
        {
            Items = new List<T>();
        }

        public IEnumerable<T> Items { get; set; }

        public PaginationMetaData MetaData { get; set; }
    }
}