using System;
using System.Threading.Tasks;

using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRepository : IDapperGenericRepository<IUser, Guid>
    {
        Task<IUser> FindByNormalizedUserNameAsync(string normalizedUserName);

        Task<IUser> FindByNormalizedEmailAsync(string normalizedEmail);

        Task<IUser> FindByKeyAsync(Guid key, IOptionsParameters options);

        Task<IPagedEnumerable<IUser>> FindAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options);
    }
}