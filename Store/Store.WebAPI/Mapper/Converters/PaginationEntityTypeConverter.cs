using AutoMapper;
using X.PagedList;
using System.Linq;
using System.Collections.Generic;

using Store.Models.Api;

namespace Store.WebAPI.Mapper.Converters
{
    public class PaginationEntityTypeConverter<TSource, TDestination> : ITypeConverter<IPagedList<TSource>, PaginationEntity<TDestination>>
    {
        public PaginationEntity<TDestination> Convert(IPagedList<TSource> source, PaginationEntity<TDestination> destination, ResolutionContext context)
        {
            return new PaginationEntity<TDestination>()
            {
                Items = context.Mapper.Map<IEnumerable<TDestination>>(source.AsEnumerable()),
                MetaData = context.Mapper.Map<PaginationMetaData>(source.GetMetaData())
            };
        }
    }
}