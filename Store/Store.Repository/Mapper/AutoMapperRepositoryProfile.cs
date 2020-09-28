using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.Entities;

namespace Store.Repository.Mapper
{
    // TODO list:
    // * resolve Identity mappings
    public class AutoMapperRepositoryProfile : Profile
    {
        public AutoMapperRepositoryProfile()
        {
            // Bookstore and Book mappings
            CreateMap<IBookstore, BookstoreEntity>().ReverseMap();
            CreateMap<IBookstore, BookstoreDto>().ReverseMap();

            CreateMap<IBook, BookEntity>().ReverseMap();
            CreateMap<IBook, BookDto>().ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destinations
            CreateMap<BookstoreEntity, BookstoreDto>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count))
                    .ReverseMap();
            CreateMap<BookEntity, BookDto>()
                    .ForMember(dst => dst.Bookstore, opt => opt.ExplicitExpansion())
                    .ReverseMap();

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