using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleRepository : IIdentityRepository<IRole, Guid>
    {
        Task<IRole> FindByNameAsync(string roleName);

        Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames);

        Task<IRole> FindByKeyAsync(Guid key, IOptionsParameters options);

        Task<IPagedList<IRole>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);
        
        Task<IPagedEnumerable<IRole>> FindWithPoliciesAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting);
    }
}