using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IRoleClaimRepository : IRepository<IRoleClaim, Guid>
    {
        Task<IEnumerable<IRoleClaim>> FindByRoleIdAsync(Guid roleId);

        Task<IPagedList<IRoleClaim>> FindAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);

        Task DeleteAsync(IRoleClaimFilteringParameters filter);
    }
}