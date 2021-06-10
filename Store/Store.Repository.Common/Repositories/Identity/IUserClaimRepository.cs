using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserClaimRepository : IRepository<IUserClaim, Guid>
    {
        Task<IEnumerable<IUserClaim>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<IUser>> FindUsersByClaimAsync(string claimType, string claimValue);

        Task<IPagedList<IUserClaim>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}