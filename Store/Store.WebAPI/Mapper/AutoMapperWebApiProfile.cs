using AutoMapper;
using X.PagedList;

using Store.Models.Api;
using Store.Models.Api.Book;
using Store.Model.Common.Models;
using Store.WebAPI.Mapper.Converters;

namespace Store.WebAPI.Mapper
{
    // TODO - resolve other mappings
    public class AutoMapperWebApiProfile : Profile
    {
        public AutoMapperWebApiProfile()
        {
            // Create maps for bookstore models
            //CreateMap<BookstoreGetApiModel, IBookstore>().ReverseMap();
            //CreateMap<BookstoreApiPostModel, IBookstore>().ReverseMap();
            //CreateMap<BookstorePatchApiModel, IBookstore>().ReverseMap();
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
            //CreateMap<SearchItemGetApiModel, ISearchItem>().ReverseMap();

            //// Create maps for identity
            //CreateMap<UserPatchApiModel, IIdentityUser>().ForMember(dst => dst.Roles, opt => opt.Ignore()); // ignore roles as they will be saved separately

            //CreateMap<UserGetApiModel, IIdentityUser>();
            //CreateMap<IIdentityUser, UserGetApiModel>().ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToArray()));

            //CreateMap<RegisterBindingApiModel, IIdentityUser>()
            //    .ForMember(dst => dst.Roles, opt => opt.Ignore())
            //    .ConstructUsing(src => new User())
            //    .ReverseMap();

            //CreateMap<RoleApiModel, IIdentityRole>()
            //    .ConstructUsing(src => new Role())
            //    .ReverseMap();

            //CreateMap<RefreshTokenApiModel, IRefreshToken>().ReverseMap();

            // Create maps for paging
            CreateMap<IPagedList, PaginationMetaData>();
            CreateMap(typeof(IPagedList<>), typeof(PaginationEntity<>)).ConvertUsing(typeof(PaginationEntityTypeConverter<,>));
        }
    }
}