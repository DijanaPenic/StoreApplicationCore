using System;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperUnitOfWork : IDisposable
    {
        IRoleRepository RoleRepository { get; }

        IRoleClaimRepository RoleClaimRepository { get; }

        IUserRepository UserRepository { get; }

        IUserClaimRepository UserClaimRepository { get; }

        IUserLoginRepository UserLoginRepository { get; }

        IDapperGenericRepository<IUserToken, IUserTokenKey> UserTokenRepository { get; }

        IUserRoleRepository UserRoleRepository { get; }

        void Commit();
    }
}