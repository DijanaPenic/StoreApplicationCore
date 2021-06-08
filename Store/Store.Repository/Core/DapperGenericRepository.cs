using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using static Dapper.SqlMapper;

namespace Store.Repository.Core
{
    internal abstract partial class GenericRepository
    {
        protected Task<T> ExecuteQueryScalarAsync<T>(string sql, object param = null)
        {
            return DbContext.Connection.ExecuteScalarAsync<T>(sql, param, DbContext.Transaction);
        }

        protected Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
        {
            return DbContext.Connection.QuerySingleOrDefaultAsync<T>(sql, param, DbContext.Transaction);
        }

        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return DbContext.Connection.QueryAsync<T>(sql, param, DbContext.Transaction);
        }

        protected Task<int> ExecuteQueryAsync(string sql, object param = null)
        {
            return DbContext.Connection.ExecuteAsync(sql, param, DbContext.Transaction);
        }

        protected Task<GridReader> QueryMultipleAsync(string sql, object param = null)
        {
            return DbContext.Connection.QueryMultipleAsync(sql, param);
        }
    }
}