using System;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRoleRepository
    {
        void Add(Guid UserId, string roleName);

        void Delete(Guid userId, string roleName);

        IEnumerable<string> GetRoleNamesByUserId(Guid userId);

        IEnumerable<IUser> GetUsersByRoleName(string roleName);
    }
}