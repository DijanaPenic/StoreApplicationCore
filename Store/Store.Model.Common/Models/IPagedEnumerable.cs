using System.Collections.Generic;

namespace Store.Model.Common.Models
{
    public interface IPagedEnumerable<T> : IEnumerable<T>
    {
        int TotalCount { get; }

        int PageSize { get; }

        int PageNumber { get; }

        int PageCount { get; }
    }
}