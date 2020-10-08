using System;

using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperUnitOfWork : IDisposable
    {
        IRoleRepository RoleRepository { get; }

        //IRoleClaimRepository RoleClaimRepository { get; }

        //IUserRepository UserRepository { get; }

        //IUserClaimRepository UserClaimRepository { get; }

        //IUserLoginRepository UserLoginRepository { get; }

        //IRepository<UserToken, UserTokenKey> UserTokenRepository { get; }

        //IUserRoleRepository UserRoleRepository { get; }

        void Commit();
    }
}