using AutoMapper;

using Store.Models;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Entities;
using Store.Entities.Identity;

namespace Store.Repository.Mapper
{
    public class AutoMapperRepositoryProfile : Profile
    {
        public AutoMapperRepositoryProfile()
        {
            // Bookstore and Book mappings
            CreateMap<IBookstore, BookstoreEntity>().ReverseMap();
            CreateMap<IBookstore, BookstoreDTO>().ReverseMap();

            CreateMap<IBook, BookEntity>().ReverseMap();
            CreateMap<IBook, BookDTO>().ReverseMap();

            // Need DTO objects because AutoMapper projection doesn't work for interface destinations
            CreateMap<BookstoreEntity, BookstoreDTO>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count))
                    .ReverseMap();
            CreateMap<BookEntity, BookDTO>()
                    .ForMember(dst => dst.Bookstore, opt => opt.ExplicitExpansion())
                    .ReverseMap();

            // Identity mappings 
            CreateMap<IRoleClaim, RoleClaimEntity>().ReverseMap();
            CreateMap<IUserToken, UserTokenEntity>().ReverseMap();
            CreateMap<IUserRefreshToken, UserRefreshTokenEntity>().ReverseMap();
            CreateMap<IUserLogin, UserLoginEntity>().ReverseMap();
            CreateMap<IUserClaim, UserClaimEntity>().ReverseMap();
            CreateMap<IUser, UserEntity>().ReverseMap();
            CreateMap<IRole, RoleEntity>()
                .ForMember(dst => dst.Claims, opt => opt.MapFrom(src => src.Policies))
                .ReverseMap();
            CreateMap<IClient, ClientEntity>().ReverseMap();

            // Email Template mappings
            CreateMap<IEmailTemplate, EmailTemplateEntity>().ReverseMap();
        }
    }
}