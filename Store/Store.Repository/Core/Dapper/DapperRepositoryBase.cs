using Dapper;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        protected Task<T> ExecuteScalarAsync<T>(string sql, object param)
        {
            return Connection.ExecuteScalarAsync<T>(sql, param, _transaction);
        }

        protected Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param)
        {
            return Connection.QuerySingleOrDefaultAsync<T>(sql, param, _transaction);
        }

        protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            return Connection.QueryAsync<T>(sql, param, _transaction);
        }

        protected Task<int> ExecuteAsync(string sql, object param)
        {
            return Connection.ExecuteAsync(sql, param, _transaction);
        }

        protected Task<GridReader> QueryMultipleAsync(string sql, object param = null)
        {
            return Connection.QueryMultipleAsync(sql, param);
        }
    }
}