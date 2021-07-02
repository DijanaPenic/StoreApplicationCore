using AutoMapper;

using Store.Model.Common.Models;
using Store.WebAPI.Models.GlobalSearch;

namespace Store.WebAPI.Mapper.Profiles
{
    public class GlobalSearchAutoMapperWebApiProfile : Profile
    {
        public GlobalSearchAutoMapperWebApiProfile()
        {
            // Create maps for global search models
            CreateMap<SearchItemGetApiModel, ISearchItem>().ReverseMap();
        }
    }
}