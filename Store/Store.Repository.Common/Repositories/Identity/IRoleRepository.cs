using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleRepository : IDapperGenericRepository<IRole, Guid>
    {
        Task<IRole> FindByNameAsync(string roleName);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames);

        Task<IPagedEnumerable<IRole>> FindAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize);
    }
}