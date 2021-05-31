using Dapper;
using System;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using static Dapper.SqlMapper;

using Store.DAL.Context;
using Store.Repository.Common.Models;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;

namespace Store.Repository.Core.Dapper
{
    internal abstract class DapperRepositoryBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DapperRepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            return _dbContext.Connection.ExecuteScalarAsync<T>(sql, param, _dbContext.Transaction);
        }

        protected Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null)
        {
            return _dbContext.Connection.QuerySingleOrDefaultAsync<T>(sql, param, _dbContext.Transaction);
        }

        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return _dbContext.Connection.QueryAsync<T>(sql, param, _dbContext.Transaction);
        }

        protected Task<int> ExecuteAsync(string sql, object param = null)
        {
            return _dbContext.Connection.ExecuteAsync(sql, param, _dbContext.Transaction);
        }

        protected Task<GridReader> QueryMultipleAsync(string sql, object param = null)
        {
            return _dbContext.Connection.QueryMultipleAsync(sql, param);
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
            return await _dbContext.Connection.QueryMultipleAsync(sql.ToString(), (object)parameters);  
        }
    }
}