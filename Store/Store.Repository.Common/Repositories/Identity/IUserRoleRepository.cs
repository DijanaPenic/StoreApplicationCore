using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRoleRepository
    {
        Task AddAsync(Guid UserId, string roleName);

        Task DeleteAsync(Guid userId, string roleName);

        Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(Guid userId);

        Task<IEnumerable<IUser>> GetUsersByRoleNameAsync(string roleName);
    }
}