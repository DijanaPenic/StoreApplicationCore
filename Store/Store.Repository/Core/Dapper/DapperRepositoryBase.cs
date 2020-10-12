using Dapper;
using System.Data;
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

        protected T ExecuteScalar<T>(string sql, object param)
        {
            return Connection.ExecuteScalar<T>(sql, param, _transaction);
        }

        protected T QuerySingleOrDefault<T>(string sql, object param)
        {
            return Connection.QuerySingleOrDefault<T>(sql, param, _transaction);
        }

        protected IEnumerable<T> Query<T>(string sql, object param = null)
        {
            return Connection.Query<T>(sql, param, _transaction);
        }

        protected void Execute(string sql, object param)
        {
            Connection.Execute(sql, param, _transaction);
        }

        protected GridReader QueryMultiple(string sql, object param = null)
        {
            return Connection.QueryMultiple(sql, param);
        }
    }
}