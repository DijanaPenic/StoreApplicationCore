using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity.Stores
{
    public interface IApplicationLoginUserStore : IUserLoginStore<IUser>, IDisposable
    {
        Task AddLoginAsync(IUser user, UserLoginInfo login, string token, CancellationToken cancellationToken);

        Task UpdateLoginAsync(IUserLogin login, CancellationToken cancellationToken);

        Task<IUserLogin> FindLoginAsync(UserLoginInfo login, CancellationToken cancellationToken);

        Task<IUserLogin> FindLoginAsync(IUser user, string token, CancellationToken cancellationToken);

        Task<IUser> FindByLoginAsync(UserLoginInfo login, bool loginConfirmed, CancellationToken cancellationToken);

        Task ConfirmLoginAsync(IUserLogin login, CancellationToken cancellationToken);

        Task<IList<UserLoginInfo>> FindLoginsAsync(IUser user, bool loginConfirmed, CancellationToken cancellationToken);
    }
}