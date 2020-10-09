using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRoleRepository
    {
        void Add(string UserId, string roleName);

        void Delete(string userId, string roleName);

        IEnumerable<string> GetRoleNamesByUserId(string userId);

        IEnumerable<IUser> GetUsersByRoleName(string roleName);
    }
}