using Dapper;
using Npgsql;
using System;
using System.Data;

using Store.Repositories.Identity;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Core.Dapper
{
    public class DapperUnitOfWork : IDapperUnitOfWork
    {
        #region Fields

        private IDbConnection _connection;

        private IDbTransaction _transaction;

        private IRoleRepository _roleRepository;

        private IRoleClaimRepository _roleClaimRepository;

        private IUserRepository _userRepository;

        private IUserClaimRepository _userClaimRepository;

        private IUserLoginRepository _userLoginRepository;

        private IDapperGenericRepository<IUserToken, IUserTokenKey> _userTokenRepository;

        private IUserRoleRepository _userRoleRepository;

        private bool _disposed;

        #endregion

        public DapperUnitOfWork(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        #region IUnitOfWork Members

        public IRoleRepository RoleRepository
        {
            get
            {
                return _roleRepository ??= new RoleRepository(_transaction);
            }
        }

        public IRoleClaimRepository RoleClaimRepository
        {
            get
            {
                return _roleClaimRepository ??= new RoleClaimRepository(_transaction);
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ??= new UserRepository(_transaction);
            }
        }

        public IUserClaimRepository UserClaimRepository
        {
            get
            {
                return _userClaimRepository ??= new UserClaimRepository(_transaction);
            }
        }

        public IUserLoginRepository UserLoginRepository
        {
            get
            {
                return _userLoginRepository ??= new UserLoginRepository(_transaction);
            }
        }

        public IDapperGenericRepository<IUserToken, IUserTokenKey> UserTokenRepository
        {
            get
            {
                return _userTokenRepository ??= new UserTokenRepository(_transaction);
            }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get
            {
                return _userRoleRepository ??= new UserRoleRepository(_transaction);
            }
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                ResetRepositories();
                _transaction = _connection.BeginTransaction();
            }
        }

        // TODO - check dispose (I'll use DI)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Private Methods

        private void ResetRepositories()
        {
            _roleRepository = null;
            _roleClaimRepository = null;
            _userRepository = null;
            _userClaimRepository = null;
            _userLoginRepository = null;
            _userTokenRepository = null;
            _userRoleRepository = null;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~DapperUnitOfWork()
        {
            Dispose(false);
        }

        #endregion
    }
}