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
        // Need DTO objects because AutoMapper projection doesn't work for interface destinations
        public AutoMapperRepositoryProfile()
        {
            // Bookstore mappings
            CreateMap<IBookstore, BookstoreEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BookstoreEntity, BookstoreExtendedDTO>()
                    .ForMember(dst => dst.Books, opt => opt.ExplicitExpansion())
                    .ForMember(dst => dst.BooksCount, opt => opt.MapFrom(src => src.Books.Count));

            // Book mappings
            CreateMap<IBook, BookEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BookEntity, BookDTO>();

            // Identity mappings 
            CreateMap<IRoleClaim, RoleClaimEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUserToken, UserTokenEntity>()
                .ForMember(dst => dst.UserId, opt => opt.Ignore())
                .ForMember(dst => dst.LoginProvider, opt => opt.Ignore())
                .ForMember(dst => dst.Name, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUserRefreshToken, UserRefreshTokenEntity>()
                .ForMember(dst => dst.UserId, opt => opt.Ignore())
                .ForMember(dst => dst.ClientId, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUserLogin, UserLoginEntity>()
                .ForMember(dst => dst.LoginProvider, opt => opt.Ignore())
                .ForMember(dst => dst.ProviderKey, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUserClaim, UserClaimEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IUser, UserEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<IRole, RoleEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ForMember(dst => dst.Claims, opt => opt.MapFrom(src => src.Policies))
                .ReverseMap();

            CreateMap<IClient, ClientEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();

            // Email Template mappings
            CreateMap<IEmailTemplate, EmailTemplateEntity>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.DateCreatedUtc, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}