using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationUserStore : IUserStore<IUser>, IDisposable
    {
        Task<IPagedEnumerable<IUser>> FindUsersAsync(string searchString, bool showInactive, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, CancellationToken cancellationToken, params string[] includeProperties);

        Task<IUser> FindUserByIdAsync(Guid id, CancellationToken cancellationToken, params string[] includeProperties);

        Task ApproveUserAsync(IUser user, CancellationToken cancellationToken);

        Task DisapproveUserAsync(IUser user, CancellationToken cancellationToken);
    }
}