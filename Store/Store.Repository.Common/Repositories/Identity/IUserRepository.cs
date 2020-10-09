using System;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRepository : IDapperGenericRepository<IUser, Guid>
    {
        IUser FindByNormalizedUserName(string normalizedUserName);

        IUser FindByNormalizedEmail(string normalizedEmail);
    }
}