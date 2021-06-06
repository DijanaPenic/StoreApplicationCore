using Dapper;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models;
using Store.Model.Common.Models;
using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Repository.Queries;
using Store.Repository.Common.Repositories;

namespace Store.Repositories
{
    internal class GlobalSearchRepository : GenericRepository, IGlobalSearchRepository
    {
        public GlobalSearchRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<IEnumerable<ISearchItem>> FindAsync(IGlobalFilteringParameters filter)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { SqlQueries.GlobalSearch.SearchAllModules.Parameters.SearchPhrase, filter.SearchString },
                { SqlQueries.GlobalSearch.SearchAllModules.Parameters.HasBookstoreReadAccess, filter.SearchTypes.Contains(SectionType.Bookstore) },
                { SqlQueries.GlobalSearch.SearchAllModules.Parameters.HasBookReadAccess, filter.SearchTypes.Contains(SectionType.Book) }
            };

            return await QueryAsync<SearchItem>(
                sql: SqlQueries.GlobalSearch.SearchAllModules.CommandText,
                param: new DynamicParameters(parameters)
            );
        }
    }
}