using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models;
using Store.Common.Parameters.Filtering;

namespace Store.Repository.Common.Repositories
{
    public interface IGlobalSearchRepository
    {
        Task<IEnumerable<ISearchItem>> FindAsync(IGlobalFilteringParameters filter);
    }
}