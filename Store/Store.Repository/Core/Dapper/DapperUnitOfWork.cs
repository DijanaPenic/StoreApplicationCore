using Dapper;
using Npgsql;
using System.Data;

using Store.Repositories.Identity;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

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

        private IUserTokenRepository _userTokenRepository;

        private IUserRoleRepository _userRoleRepository;

        private IUserRefreshTokenRepository _userRefreshTokenRepository;

        private IClientRepository _clientRepository;

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

        public IUserTokenRepository UserTokenRepository
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

        public IUserRefreshTokenRepository UserRefreshTokenRepository
        {
            get
            {
                return _userRefreshTokenRepository ??= new UserRefreshTokenRepository(_transaction);
            }
        }

        public IClientRepository ClientRepository
        {
            get
            {
                return _clientRepository ??= new ClientRepository(_transaction);
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
                // Transaction is closed after a commit. 
                _transaction.Dispose();

                // Transaction resources have been released so we need to re-create UoW repository classes, as they're using reference to disposed transaction.
                ResetRepositories();

                _transaction = _connection.BeginTransaction();
            }
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
            _userRefreshTokenRepository = null;
            _clientRepository = null;
        }

        #endregion
    }
}