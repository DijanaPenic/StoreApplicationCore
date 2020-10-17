using AutoMapper;
using X.PagedList;
using System.Linq;
using System.Collections.Generic;

using Store.Models.Api;

namespace Store.WebAPI.Mapper.Converters
{
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, PagedResponse<TDestination>>
    {
        public PagedResponse<TDestination> Convert(IPagedList<TSource> source, PagedResponse<TDestination> destination, ResolutionContext context)
        {
            return new PagedResponse<TDestination>()
            {
                Items = context.Mapper.Map<IEnumerable<TDestination>>(source.AsEnumerable()),
                MetaData = context.Mapper.Map<PagedResponseMetaData>(source.GetMetaData())
            };
        }
    }
}