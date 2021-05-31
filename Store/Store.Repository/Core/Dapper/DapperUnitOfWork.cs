using Dapper;
using AutoMapper;
using System.Threading.Tasks;

using Store.DAL.Context;
using Store.Repositories;
using Store.Repositories.Identity;
using Store.Repository.Common.Core.Dapper;
using Store.Repository.Common.Repositories;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Core.Dapper
{
    internal class DapperUnitOfWork : IDapperUnitOfWork
    {
        #region Fields

        private readonly ApplicationDbContext _dbContext;

        private readonly IMapper _mapper;

        private IRoleRepository _roleRepository;

        private IRoleClaimRepository _roleClaimRepository;

        private IUserRepository _userRepository;

        private IUserClaimRepository _userClaimRepository;

        private IUserLoginRepository _userLoginRepository;

        private IUserTokenRepository _userTokenRepository;

        private IUserRoleRepository _userRoleRepository;

        private IUserRefreshTokenRepository _userRefreshTokenRepository;

        private IClientRepository _clientRepository;

        private IBookRepository _bookRepository;

        private IBookstoreRepository _bookstoreRepository;

        private IEmailTemplateRepository _emailTemplateRepository;

        #endregion

        // TOOD - what needs to be disposed? 
        // 1 - track opened connections to database
        // 2 - 
        public DapperUnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
        {
            dbContext.Connection.Open();
            dbContext.Transaction = dbContext.Connection.BeginTransaction();
            _dbContext = dbContext;

            _mapper = mapper;

            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        #region IUnitOfWork Members

        public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_dbContext);

        public IRoleClaimRepository RoleClaimRepository => _roleClaimRepository ??= new RoleClaimRepository(_dbContext);

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext);

        public IUserClaimRepository UserClaimRepository => _userClaimRepository ??= new UserClaimRepository(_dbContext);

        public IUserLoginRepository UserLoginRepository => _userLoginRepository ??= new UserLoginRepository(_dbContext);

        public IUserTokenRepository UserTokenRepository => _userTokenRepository ??= new UserTokenRepository(_dbContext);

        public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_dbContext);

        public IUserRefreshTokenRepository UserRefreshTokenRepository => _userRefreshTokenRepository ??= new UserRefreshTokenRepository(_dbContext);

        public IClientRepository ClientRepository => _clientRepository ??= new ClientRepository(_dbContext);

        public IBookRepository BookRepository => _bookRepository ??= new BookRepository(_dbContext, _mapper);

        public IBookstoreRepository BookstoreRepository => _bookstoreRepository ??= new BookstoreRepository(_dbContext, _mapper);

        public IEmailTemplateRepository EmailTemplateRepository => _emailTemplateRepository ??= new EmailTemplateRepository(_dbContext, _mapper);

        // TODO - need to save changes on dbContext
        public async Task SaveChangesAsync()
        {
            try
            {
                _dbContext.Transaction.Commit();
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _dbContext.Transaction.Rollback();
            }
            finally
            {
                // Transaction should be disposed after commit/rollback
                _dbContext.Transaction.Dispose();

                _dbContext.Transaction = _dbContext.Connection.BeginTransaction();
            }
        }

        #endregion
    }
}