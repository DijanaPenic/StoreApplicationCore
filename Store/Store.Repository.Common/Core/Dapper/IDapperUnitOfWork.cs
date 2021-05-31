﻿using System.Threading.Tasks;

using Store.Repository.Common.Repositories;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Common.Core.Dapper
{
    public interface IDapperUnitOfWork
    {
        IRoleRepository RoleRepository { get; }

        IRoleClaimRepository RoleClaimRepository { get; }

        IUserRepository UserRepository { get; }

        IUserClaimRepository UserClaimRepository { get; }

        IUserLoginRepository UserLoginRepository { get; }

        IUserTokenRepository UserTokenRepository { get; }

        IUserRoleRepository UserRoleRepository { get; }

        IUserRefreshTokenRepository UserRefreshTokenRepository { get; }

        IClientRepository ClientRepository { get; }

        IBookRepository BookRepository { get; }

        IBookstoreRepository BookstoreRepository { get; }

        IEmailTemplateRepository EmailTemplateRepository { get; }

        Task SaveChangesAsync();
    }
}