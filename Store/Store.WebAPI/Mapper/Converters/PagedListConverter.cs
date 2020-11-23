using AutoMapper;
using X.PagedList;
using System.Linq;
using System.Collections.Generic;

using Store.WebAPI.Models;

namespace Store.WebAPI.Mapper.Converters
{
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, PagedApiResponse<TDestination>>
    {
        public PagedApiResponse<TDestination> Convert(IPagedList<TSource> source, PagedApiResponse<TDestination> destination, ResolutionContext context)
        {
            return new PagedApiResponse<TDestination>()
            {
                Items = context.Mapper.Map<IEnumerable<TDestination>>(source.AsEnumerable()),
                MetaData = context.Mapper.Map<PagedResponseMetaData>(source.GetMetaData())
            };
        }
    }
}