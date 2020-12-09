using AutoMapper;
using System.Linq;
using X.PagedList;
using Microsoft.AspNetCore.Identity;

using Store.WebAPI.Models;
using Store.WebAPI.Models.Book;
using Store.WebAPI.Models.Identity;
using Store.WebAPI.Models.Bookstore;
using Store.WebAPI.Models.GlobalSearch;
using Store.WebAPI.Mapper.Converters;
using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Mapper.Profiles
{
    // TODO - resolve other mappings
    public class AutoMapperWebApiProfile : Profile
    {
        public AutoMapperWebApiProfile()
        {
            // Create maps for bookstore models
            CreateMap<BookstoreGetApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstorePostApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstorePatchApiModel, IBookstore>().ReverseMap();
            CreateMap<BookstoreApiModel, IBookstore>().ReverseMap();

            // Create maps for book models
            CreateMap<BookGetApiModel, IBook>().ReverseMap();
            CreateMap<BookPostApiModel, IBook>().ReverseMap();
            CreateMap<BookPatchApiModel, IBook>().ReverseMap();

            //// Create maps for reporting models
            //CreateMap<BookstoreReportPOCO, IBookstore>()
            //    .ForMember(dst => dst.DateCreatedUtc, opt => opt.MapFrom(src => src.DateCreated))
            //    .ForMember(dst => dst.DateUpdatedUtc, opt => opt.MapFrom(src => src.DateUpdated))
            //    .ReverseMap();

            //CreateMap<BookReportPOCO, IBook>()
            //    .ForMember(dst => dst.DateCreatedUtc, opt => opt.MapFrom(src => src.DateCreated))
            //    .ForMember(dst => dst.DateUpdatedUtc, opt => opt.MapFrom(src => src.DateUpdated))
            //    .ReverseMap();

            // Create maps for global search models
            CreateMap<SearchItemGetApiModel, ISearchItem>().ReverseMap();

            // Create maps for identity models
            CreateMap<UserPatchApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore()); // ignore roles as they will be saved separately

            CreateMap<UserGetApiModel, IUser>();
            CreateMap<IUser, UserGetApiModel>().ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToArray()));

            CreateMap<UserCreatePostApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore());
            CreateMap<RegisterPostApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore());

            CreateMap<RoleGetApiModel, IRole>().ConstructUsing(src => new Role()).ReverseMap();

            CreateMap<UserLoginInfo, ExternalLoginGetApiModel>();

            // Create maps for paging
            CreateMap<IPagedList, PagedResponseMetaData>();
            CreateMap(typeof(IPagedList<>), typeof(PagedApiResponse<>)).ConvertUsing(typeof(PagedListConverter<,>));

            CreateMap(typeof(IPagedEnumerable<>), typeof(PagedApiResponse<>)).ConvertUsing(typeof(PagedEnumerableConverter<,>));
        }
    }
}