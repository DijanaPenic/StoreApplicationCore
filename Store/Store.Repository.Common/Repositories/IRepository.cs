using X.PagedList;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common
{
    public interface IRepository<TModel, TKey> where TModel : class
    {
        Task<IEnumerable<TModel>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null);

        Task<TModel> FindByKeyAsync(TKey key, IOptionsParameters options = null);

        Task<ResponseStatus> AddAsync(TModel model);

        Task<ResponseStatus> UpdateAsync(TModel model);

        Task<ResponseStatus> DeleteByKeyAsync(TKey key);
    }
}