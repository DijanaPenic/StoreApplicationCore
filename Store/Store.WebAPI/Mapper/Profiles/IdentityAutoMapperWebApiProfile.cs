using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Identity;

using Store.WebAPI.Models.Identity;
using Store.WebAPI.Mapper.Converters;
using Store.Model.Common.Models.Identity;

namespace Store.WebAPI.Mapper.Profiles
{
    public class IdentityAutoMapperWebApiProfile : Profile
    {
        public IdentityAutoMapperWebApiProfile()
        {
            CreateMap<UserPatchApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore()); // ignore roles as they will be saved separately
            CreateMap<UserProfilePatchApiModel, IUser>().ReverseMap();

            CreateMap<UserLoginInfo, ExternalLoginGetApiModel>();

            CreateMap<UserGetApiModel, IUser>();
            CreateMap<IUser, UserGetApiModel>().ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToArray()));

            CreateMap<UserPostApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore());  // ignore roles as they will be saved separately
            CreateMap<RegisterPostApiModel, IUser>().ForMember(dst => dst.Roles, opt => opt.Ignore()); // ignore roles as they will be saved separately

            CreateMap<IUserRefreshToken, UserRefreshTokenGetApiModel>();
            CreateMap<IUserClaim, UserClaimGetApiModel>();
            CreateMap<IUserLogin, UserLoginGetApiModel>();
            CreateMap<IUserToken, UserTokenGetApiModel>();

            // Create maps for role models
            CreateMap(typeof(IRole), typeof(RoleGetApiModel)).ConvertUsing(typeof(PolicyConverter<IRole, RoleGetApiModel>));
            CreateMap<RolePostApiModel, IRole>();
            CreateMap<RolePatchApiModel, IRole>();

            // Create maps for permissions
            CreateMap<AccessActionPostModel, IAccessAction>();
        }
    }
}