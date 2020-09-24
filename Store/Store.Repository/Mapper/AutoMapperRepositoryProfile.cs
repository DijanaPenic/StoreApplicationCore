using AutoMapper;

using Store.Entities;
using Store.Model.Common.Models;

namespace Store.Repository.Mapper
{
    // TODO list:
    // * resolve Identity mappings
    // * resolve DTO mappings
    public class AutoMapperRepositoryProfile : Profile
    {
        public AutoMapperRepositoryProfile()
        {
            // Bookstore and Book mappings
            CreateMap<IBookstore, BookstoreEntity>().ReverseMap();
            //CreateMap<IBookstore, BookstoreDTO>().ReverseMap();

            CreateMap<IBook, BookEntity>().ReverseMap();
            //CreateMap<IBook, BookTDO>().ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destination
            //CreateMap<BookstoreEntity, BookstoreDTO>()
                    //.ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    //.ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count))
                    //.ReverseMap();
            //CreateMap<BookEntity, BookTDO>()
            //        .ForMember(dst => dst.Bookstore, opt => opt.ExplicitExpansion())
            //        .ReverseMap();

            // Identity mappings
            //CreateMap<UserEntity, IIdentityUser>();
            //CreateMap<IIdentityUser, UserEntity>()
            //    .ForMember(dst => dst.Claims, opt => opt.Ignore())
            //    .ForMember(dst => dst.Logins, opt => opt.Ignore())
            //    .ForMember(dst => dst.Roles, opt => opt.Ignore());

            //CreateMap<RoleEntity, IIdentityRole>();
            //CreateMap<IIdentityRole, RoleEntity>().ForMember(dst => dst.Users, opt => opt.Ignore());

            //CreateMap<IClaim, ClaimEntity>().ReverseMap();

            //CreateMap<IExternalLogin, ExternalLoginEntity>().ReverseMap();

            //CreateMap<IClient, ClientEntity>().ReverseMap();

            //CreateMap<IRefreshToken, RefreshTokenEntity>().ReverseMap();

        }
    }
}