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
            CreateMap<IBookstore, BookstoreEntity>()
                .ForMember(dst => dst.Books, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BookstoreEntity, BookstoreExtendedDto>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count));

            // Book mappings
            CreateMap<IBook, BookEntity>()
                .ForMember(dst => dst.Bookstore, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BookEntity, BookDto>();

            // Identity mappings 
            CreateMap<IRoleClaim, RoleClaimEntity>()
                .ForMember(dst => dst.Role, opt => opt.Ignore())
                .ReverseMap();

            // NOTE: If using an interface as a destination type AutoMapper will dynamically create an implementation (proxy) type.
            // However, the proxy generation only supports properties. As a workaround we can explicitly specify how the destination objects 
            // should be constructed with using one of the ConstructUsing overloads. In that case AutoMapper does not generate proxies.
            CreateMap<UserTokenEntity, IUserToken>()
                .ForSourceMember(src => src.User, opt => opt.DoNotValidate())
                .ConstructUsing(entity => new UserToken())
                .ReverseMap();

            CreateMap<UserRefreshTokenEntity, IUserRefreshToken>()
                .ForSourceMember(src => src.User, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.Client, opt => opt.DoNotValidate())
                .ConstructUsing(entity => new UserRefreshToken())
                .ReverseMap();

            CreateMap<IUserLogin, UserLoginEntity>()
                .ForMember(dst => dst.User, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUserClaim, UserClaimEntity>()
                .ForMember(dst => dst.User, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUser, UserEntity>()
                .ForMember(dst => dst.Claims, opt => opt.Ignore())
                .ForMember(dst => dst.Roles, opt => opt.Ignore())
                .ForMember(dst => dst.RefreshTokens, opt => opt.Ignore())
                .ForMember(dst => dst.UserTokens, opt => opt.Ignore())
                .ForMember(dst => dst.Logins, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IRole, RoleEntity>()
                .ForMember(dst => dst.Claims, opt => opt.Ignore())
                .ForMember(dst => dst.Users, opt => opt.Ignore());
            CreateMap<RoleEntity, IRole>()
                .ForMember(dst => dst.Policies, opt => opt.MapFrom(src => src.Claims));

            CreateMap<IClient, ClientEntity>().ReverseMap();

            // Email Template mappings
            CreateMap<IEmailTemplate, EmailTemplateEntity>()
                .ForMember(dst => dst.Client, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}