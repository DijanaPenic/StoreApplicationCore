using System;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;

using Store.Common.Extensions;

using static Dapper.SqlMapper;
using System.Dynamic;

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

        protected async Task<GridReader> FindAsync(string tableName, string tableAlias, string selectAlias, string filterExpression, string includeExpression, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, dynamic searchParameters)
        {
            // Prepare query parameters
            dynamic parameters = searchParameters ?? new ExpandoObject();

            parameters.PageSize = pageSize;
            parameters.Offset = (pageNumber - 1) * pageSize;
            parameters.SortOrderProperty = $"{tableAlias}.{sortOrderProperty}".ToSnakeCase();

            string order = $"ORDER BY @{nameof(parameters.SortOrderProperty)} {((isDescendingSortOrder) ? "DESC" : "ASC")}";

            // Set query base
            StringBuilder sql = new StringBuilder();
            sql.Append(@$"SELECT {selectAlias} FROM
                          (
                              SELECT * FROM {tableName} {tableAlias}
                              {filterExpression}
                              {order}
                              OFFSET @{nameof(parameters.Offset)} ROWS
                              FETCH NEXT @{nameof(parameters.PageSize)} ROWS ONLY
                          ) as {tableAlias}");
            sql.Append(Environment.NewLine);

            // Set prefetch and order
            sql.Append(includeExpression);
            sql.Append(Environment.NewLine);

            sql.Append($"{order};");
            sql.Append(Environment.NewLine);

            // Check total count
            sql.Append(@$"SELECT COUNT(*) FROM {tableName} {filterExpression}");

            // Get results from the database and prepare response model
            return await Connection.QueryMultipleAsync(sql.ToString(), (object)parameters);  
        }
    }
}