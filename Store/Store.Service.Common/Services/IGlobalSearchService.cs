using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Common.Models;
using Store.Common.Parameters.Filtering;

namespace Store.Service.Common.Services
{
    public interface IGlobalSearchService
    {
        Task<IEnumerable<ISearchItem>> FindAsync(IGlobalFilteringParameters filtering);
    }
}