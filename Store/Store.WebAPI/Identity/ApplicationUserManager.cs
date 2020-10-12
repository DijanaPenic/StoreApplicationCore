using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;
using Store.Service.Common.Services.Identity;

namespace Store.WebAPI.Identity
{
    public sealed class ApplicationUserManager : UserManager<IUser>
    {
        private readonly IUserFilterStore<IUser> _userFilterStore;

        public ApplicationUserManager(
            IUserStore<IUser> userStore, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<IUser> passwordHasher, 
            IEnumerable<IUserValidator<IUser>> userValidators, 
            IEnumerable<IPasswordValidator<IUser>> passwordValidators,
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<IUser>> logger) 
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userFilterStore = (IUserFilterStore<IUser>)userStore;
        }

        //public Task<IPagedList<IUser>> FindUsersAsync(string searchString, bool showInactive, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties)
        //{
        //    return _userFilterStore.FindUsersAsync(searchString, showInactive, isDescendingSortOrder, sortOrderProperty, pageNumber, pageSize, includeProperties);
        //}

        public Task<IUser> FindUserByIdAsync(Guid id, params string[] includeProperties)
        {
            return _userFilterStore.FindUserByIdAsync(id, includeProperties);
        }

        //public Task<IUser> FindUserByUserNameAsync(string userName, params string[] includeProperties)
        //{
        //    return _userFilterStore.FindUserByUserNameAsync(userName, includeProperties);
        //}
    }
}