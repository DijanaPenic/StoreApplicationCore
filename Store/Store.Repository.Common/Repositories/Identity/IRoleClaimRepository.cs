using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleClaimRepository : IIdentityRepository<IRoleClaim, Guid>
    {
        Task<IEnumerable<IRoleClaim>> FindByRoleIdAsync(Guid roleId);

        Task<IPagedList<IRoleClaim>> FindAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting);

        Task DeleteAsync(IRoleClaimFilteringParameters filter);
    }
}