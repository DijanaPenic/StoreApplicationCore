using System;
using System.Threading.Tasks;
using X.PagedList;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserRepository : IRepository<IUser, Guid>
    {
        Task<IUser> FindByNormalizedUserNameAsync(string normalizedUserName);

        Task<IUser> FindByNormalizedEmailAsync(string normalizedEmail);

        Task<IPagedList<IUser>> FindAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null);
    }
}