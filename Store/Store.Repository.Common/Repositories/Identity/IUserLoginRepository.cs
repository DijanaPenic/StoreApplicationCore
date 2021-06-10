using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using X.PagedList;

using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserLoginRepository : IRepository<IUserLogin, IUserLoginKey>
    {
        Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId);

        Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId, bool isConfirmed);

        Task<IUserLogin> FindByUserIdAsync(Guid userId, string token);

        Task<IPagedList<IUserLogin>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}