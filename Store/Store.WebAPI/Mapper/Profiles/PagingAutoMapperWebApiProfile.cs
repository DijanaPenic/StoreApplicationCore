using AutoMapper;
using X.PagedList;

using Store.WebAPI.Models;
using Store.WebAPI.Mapper.Converters;
using Store.Model.Common.Models;

namespace Store.WebAPI.Mapper.Profiles
{
    public class PagingAutoMapperWebApiProfile : Profile
    {
        public PagingAutoMapperWebApiProfile()
        {
            CreateMap<IPagedList, PagedResponseMetaData>();
            CreateMap(typeof(IPagedList<>), typeof(PagedApiResponse<>)).ConvertUsing(typeof(PagedListConverter<,>));

            CreateMap(typeof(IPagedEnumerable<>), typeof(PagedApiResponse<>)).ConvertUsing(typeof(PagedEnumerableConverter<,>));
        }
    }
}