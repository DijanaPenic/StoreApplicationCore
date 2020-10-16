using System;
using System.Threading.Tasks;

using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRepository : IDapperGenericRepository<IUser, Guid>
    {
        Task<IUser> FindByNormalizedUserNameAsync(string normalizedUserName);

        Task<IUser> FindByNormalizedEmailAsync(string normalizedEmail);

        Task<IUser> FindByKeyAsync(Guid key, params string[] includeProperties);

        Task<IUser> FindAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties);
    }
}