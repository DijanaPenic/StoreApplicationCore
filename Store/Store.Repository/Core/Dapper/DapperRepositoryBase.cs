using System;
using System.Text;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;

using static Dapper.SqlMapper;

using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Repository.Repositories.Models;

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

        protected async Task<GridReader> FindAsync(IQueryTableModel queryTableModel, IFilterModel filterModel, IPagingParameters paging, ISortingParameters sorting)
        {
            // Prepare query parameters
            dynamic parameters = filterModel.Parameters ?? new ExpandoObject();
            parameters.PageSize = paging.PageSize;
            parameters.Offset = (paging.PageNumber - 1) * paging.PageSize;

            string order = string.Empty;
            if (sorting?.Sorters != null)
            {
                IList<string> sortParameters = new List<string>();
                foreach (ISortingPair sort in sorting.Sorters)
                {
                    sortParameters.Add($"{queryTableModel.TableAlias},{sort.GetQuerySortExpression()}");
                }
                order = new StringBuilder("ORDER BY ").AppendJoin(", ", sortParameters).ToString();
            }     

            // Set query base
            StringBuilder sql = new StringBuilder();
            sql.Append(@$"SELECT {queryTableModel.SelectStatement} FROM
                          (
                              SELECT * FROM {queryTableModel.TableName} {queryTableModel.TableAlias}
                              {filterModel.Expression}
                              {order}
                              OFFSET @{nameof(parameters.Offset)} ROWS
                              FETCH NEXT @{nameof(parameters.PageSize)} ROWS ONLY
                          ) as {queryTableModel.TableAlias}");
            sql.Append(Environment.NewLine);

            // Set prefetch and order
            sql.Append(queryTableModel.IncludeStatement);
            sql.Append(Environment.NewLine);

            sql.Append($"{order};");
            sql.Append(Environment.NewLine);

            // Check total count
            sql.Append(@$"SELECT COUNT(*) FROM {queryTableModel.TableName} {queryTableModel.TableAlias} {filterModel.Expression};");

            // Get results from the database and prepare response model
            return await Connection.QueryMultipleAsync(sql.ToString(), (object)parameters);  
        }

        // TODO - remove
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
            sql.Append(@$"SELECT COUNT(*) FROM {tableName} {tableAlias} {filterExpression};");

            // Get results from the database and prepare response model
            return await Connection.QueryMultipleAsync(sql.ToString(), (object)parameters);
        }
    }
}