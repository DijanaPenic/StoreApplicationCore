using AutoMapper;
using System.Linq;
using System.Collections.Generic;

using Store.Models.Api;
using Store.Model.Common.Models;

namespace Store.WebAPI.Mapper.Converters
{
    public class PagedEnumerableConverter<TSource, TDestination> : ITypeConverter<IPagedEnumerable<TSource>, PagedResponse<TDestination>>
    {
        public PagedResponse<TDestination> Convert(IPagedEnumerable<TSource> source, PagedResponse<TDestination> destination, ResolutionContext context)
        {
            return new PagedResponse<TDestination>()
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