using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Service.Common.Services.Identity
{
    public interface IUserFilterStore<TUser> : IUserStore<TUser>, IDisposable where TUser : class, IUser
    {
        //Task<IPagedList<TUser>> FindUsersAsync(string searchString, bool showInactive, bool isDescendingSortOrder, string sortOrderProperty, int pageNumber, int pageSize, params string[] includeProperties);

        Task<TUser> FindUserByIdAsync(Guid id, params string[] includeProperties);
    }
}