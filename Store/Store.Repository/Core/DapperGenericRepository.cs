using Dapper;
using System;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using static Dapper.SqlMapper;

using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Repository.Common.Models;

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

        protected async Task<GridReader> FindQueryAsync(IQueryTableModel queryTableModel, IFilterModel filterModel, IPagingParameters paging, ISortingParameters sorting)
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
            return await DbContext.Connection.QueryMultipleAsync(sql.ToString(), (object)parameters);
        }
    }
}