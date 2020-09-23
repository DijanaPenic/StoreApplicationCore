using AutoMapper;
using X.PagedList;
using System.Collections.Generic;

namespace Store.Common.Extensions
{
    public static class PagedListExtensions
    {
        public static IPagedList<TDestination> ToPagedList<TSource, TDestination>(this IPagedList<TSource> sourceList, IMapper mapper)
        {
            IEnumerable<TDestination> destinationList = mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(sourceList);

            return new StaticPagedList<TDestination>(destinationList, sourceList.GetMetaData());
        }
    }
}