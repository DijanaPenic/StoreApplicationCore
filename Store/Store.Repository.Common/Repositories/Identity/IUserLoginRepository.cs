using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserLoginRepository : IRepository<IUserLogin, IUserLoginKey>
    {
        Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId, bool isConfirmed);

        Task<IUserLogin> FindByUserIdAsync(Guid userId, string token);
    }
}