using AutoMapper;

using Store.Models;
using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Entities;
using Store.Entities.Identity;

namespace Store.Repository.Mapper
{
    public class AutoMapperRepositoryProfile : Profile
    {
        // Need DTO objects because AutoMapper projection doesn't work for interface destinations
        public AutoMapperRepositoryProfile()
        {
            // Bookstore mappings
            CreateMap<IBookstore, BookstoreEntity>().ReverseMap();

            CreateMap<BookstoreEntity, BookstoreExtendedDTO>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count));

            // Book mappings
            CreateMap<IBook, BookEntity>().ReverseMap();

            CreateMap<BookEntity, BookDTO>();

            // Identity mappings 
            CreateMap<IRoleClaim, RoleClaimEntity>().ReverseMap();

            // NOTE: If using an interface as a destination type AutoMapper will dynamically create an implementation (proxy) type.
            // However, the proxy generation only supports properties. As a workaround we can explicitly specify how the destination objects 
            // should be constructed with using one of the ConstructUsing overloads. In that case AutoMapper does not generate proxies.
            CreateMap<UserTokenEntity, IUserToken>()
                .ConstructUsing(entity => new UserToken())
                .ReverseMap();

            CreateMap<UserRefreshTokenEntity, IUserRefreshToken>()
                .ConstructUsing(entity => new UserRefreshToken())
                .ReverseMap();

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