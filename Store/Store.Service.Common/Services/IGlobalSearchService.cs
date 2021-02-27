using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Enums;
using Store.Model.Common.Models;

namespace Store.Service.Common.Services
{
    public interface IGlobalSearchService
    {
        Task<IEnumerable<ISearchItem>> FindAsync(string searchString, IList<SectionType> searchTypes);
    }
}