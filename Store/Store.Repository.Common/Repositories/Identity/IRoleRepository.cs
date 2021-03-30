using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleRepository : IDapperGenericRepository<IRole, Guid>
    {
        Task<IRole> FindByNameAsync(string roleName);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames);

        Task<IRole> FindByKeyAsync(Guid key, IOptionsParameters options);

        Task<IPagedEnumerable<IRole>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);
        
        Task<IPagedEnumerable<IRole>> FindRolesWithPoliciesAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting);
    }
}