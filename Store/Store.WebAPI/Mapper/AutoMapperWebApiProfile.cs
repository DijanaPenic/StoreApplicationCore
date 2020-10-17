using AutoMapper;
using System.Linq;
using X.PagedList;

using Store.Models.Api;
using Store.Models.Api.Book;
using Store.Models.Api.Identity;
using Store.Models.Api.Bookstore;
using Store.Models.Api.GlobalSearch;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Model.Models.Identity;
using Store.WebAPI.Mapper.Converters;

namespace Store.WebAPI.Mapper
{
    // TODO - resolve other mappings
    public class AutoMapperWebApiProfile : Profile
    {
        public AutoMapperWebApiProfile()
        {
            // Create maps for bookstore models
            CreateMap<BookstoreGetApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstoreApiPostModel, IBookstore>().ReverseMap();
            CreateMap<BookstorePatchApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstoreApiModel, IBookstore>().ReverseMap();

            // Create maps for book models
            CreateMap<BookGetApiModel, IBook>().ReverseMap();
            CreateMap<BookPostApiModel, IBook>().ReverseMap();
            CreateMap<BookPatchApiModel, IBook>().ReverseMap();

            //// Create maps for reporting
            //CreateMap<BookstoreReportPOCO, IBookstore>()
            //    .ForMember(dst => dst.DateCreatedUtc, opt => opt.MapFrom(src => src.DateCreated))
            //    .ForMember(dst => dst.DateUpdatedUtc, opt => opt.MapFrom(src => src.DateUpdated))
            //    .ReverseMap();

            //CreateMap<BookReportPOCO, IBook>()
            //    .ForMember(dst => dst.DateCreatedUtc, opt => opt.MapFrom(src => src.DateCreated))
            //    .ForMember(dst => dst.DateUpdatedUtc, opt => opt.MapFrom(src => src.DateUpdated))
            //    .ReverseMap();

            //// Create maps for global search
            CreateMap<SearchItemGetApiModel, ISearchItem>().ReverseMap();

            //// Create maps for identity
            CreateMap<UserPatchApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore()); // ignore roles as they will be saved separately

            CreateMap<UserGetApiModel, IUser>();
            CreateMap<IUser, UserGetApiModel>().ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToArray()));

            CreateMap<RegisterApiModel, IUser>().ConstructUsing(src => new User()).ReverseMap();

            CreateMap<RoleApiModel, IRole>().ConstructUsing(src => new Role()).ReverseMap();

            // Create maps for paging
            CreateMap<IPagedList, PagedResponseMetaData>();
            CreateMap(typeof(IPagedList<>), typeof(PagedResponse<>)).ConvertUsing(typeof(PagedListConverter<,>));

            CreateMap(typeof(IPagedEnumerable<>), typeof(PagedResponse<>)).ConvertUsing(typeof(PagedEnumerableConverter<,>));
        }
    }
}