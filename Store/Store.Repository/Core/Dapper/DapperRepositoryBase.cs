using System;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;

using Store.Common.Extensions;

using static Dapper.SqlMapper;

namespace Store.Repository.Core.Dapper
{
    internal abstract class DapperRepositoryBase
    {
        private readonly IDbTransaction _transaction;

        private IDbConnection Connection { get { return _transaction.Connection; } }

        public DapperRepositoryBase(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        protected Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, _transaction);
        }

        protected Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
        {
            return Connection.QuerySingleOrDefaultAsync<T>(sql, param, _transaction);
        }

        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return Connection.QueryAsync<T>(sql, param, _transaction);
        }

        protected Task<int> ExecuteAsync(string sql, object param = null)
        {
            return Connection.ExecuteAsync(sql, param, _transaction);
        }

        protected Task<GridReader> QueryMultipleAsync(string sql, object param = null)
        {
            return Connection.QueryMultipleAsync(sql, param);
        }

        protected async Task<GridReader> FindAsync(string table, string select, string searchFilter, string include, string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize)
        {
            // Prepare query parameters
            int offset = (pageNumber - 1) * pageSize;
            sortOrderProperty = sortOrderProperty.ToSnakeCase();
            searchString = searchString?.ToLowerInvariant();

            // Set query base
            StringBuilder sql = new StringBuilder(@$"SELECT {select} FROM {table}");
            sql.Append(Environment.NewLine);

            // Set prefetch
            sql.Append(include);

            // Set filter and paging
            sql.Append($@"WHERE {searchFilter}
                          ORDER by {sortOrderProperty} {((isDescendingSortOrder) ? "DESC" : "ASC")}
                          OFFSET @{nameof(offset)} ROWS
                          FETCH NEXT @{nameof(pageSize)} ROWS ONLY;");
            sql.Append(Environment.NewLine);

            // Check total count
            sql.Append(@$"SELECT COUNT(*) FROM {table} WHERE {searchFilter}");

            // Get results from the database and prepare response model
            return await Connection.QueryMultipleAsync(sql.ToString(), param: new { searchString = $"%{searchString}%", offset, pageSize });  // TODO - fix "searchString"
        }
    }
}