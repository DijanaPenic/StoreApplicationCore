using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.Service.Common.Services.Identity
{
    public interface IApplicationUserStore<TUser> : IUserStore<TUser>, IDisposable where TUser : class, IUser
    {
        Task<IPagedEnumerable<TUser>> FindUsersAsync(string searchString, bool showInactive, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties);

        Task<TUser> FindUserByIdAsync(Guid id, params string[] includeProperties);
    }
}