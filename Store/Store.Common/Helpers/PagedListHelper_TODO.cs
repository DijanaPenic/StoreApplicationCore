using PagedList;
using System.Collections.Generic;

namespace Store.Common.Helpers
{
    public static class PagedListHelper
    {
        // Create a mutable pagedlist which indicates no results
        public static IPagedList<T> Empty<T>()
        {
            return new List<T>().ToPagedList(1, 1);
        }
    }
}