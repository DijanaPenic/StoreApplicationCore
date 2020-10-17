using System;
using System.Collections;
using System.Collections.Generic;

using Store.Model.Common.Models;

namespace Store.Model.Models
{
    public class PagedEnumerable<T> : IPagedEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public int TotalCount { get; }

        public int PageSize { get; }

        public int PageNumber { get; }

        public int PageCount => (int)Math.Ceiling(decimal.Divide(TotalCount, PageSize));

        public PagedEnumerable(IEnumerable<T> enumerable, int totalCount, int pageSize, int pageNumber)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
            _enumerable = enumerable;
        }

        public IEnumerator<T> GetEnumerator() => _enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _enumerable.GetEnumerator();
    }
}