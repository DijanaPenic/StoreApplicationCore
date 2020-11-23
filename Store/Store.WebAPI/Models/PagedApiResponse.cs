using System.Collections.Generic;

namespace Store.WebAPI.Models
{
    public class PagedResponseMetaData
    {
        public bool HasNextPage => PageNumber < PageCount;

        public bool HasPreviousPage => PageNumber > 1;

        public bool IsFirstPage => 1 == PageNumber;

        public bool IsLastPage => PageCount == PageNumber;

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }
    }

    public class PagedApiResponse<T>
    {
        public PagedApiResponse()
        {
            Items = new List<T>(); // TODO - change intialization
        }

        public IEnumerable<T> Items { get; set; }

        public PagedResponseMetaData MetaData { get; set; }
    }
}