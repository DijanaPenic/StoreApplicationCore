using AutoMapper;
using System.Linq;
using System.Collections.Generic;

using Store.WebAPI.Models;
using Store.Model.Common.Models;

namespace Store.WebAPI.Mapper.Converters
{
    public class PagedEnumerableConverter<TSource, TDestination> : ITypeConverter<IPagedEnumerable<TSource>, PagedApiResponse<TDestination>>
    {
        public PagedApiResponse<TDestination> Convert(IPagedEnumerable<TSource> source, PagedApiResponse<TDestination> destination, ResolutionContext context)
        {
            return new PagedApiResponse<TDestination>()
            {
                Items = context.Mapper.Map<IEnumerable<TDestination>>(source.AsEnumerable()),
                MetaData = new PagedResponseMetaData
                {
                    PageCount = source.PageCount,
                    PageNumber = source.PageNumber,
                    PageSize = source.PageSize,
                    TotalItemCount = source.TotalCount
                }
            };
        }
    }
}