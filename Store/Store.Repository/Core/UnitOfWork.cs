using Dapper;
using AutoMapper;
using System.Threading.Tasks;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Repositories;
using Store.Repositories.Identity;
using Store.Repository.Common.Core;
using Store.Repository.Common.Repositories;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Core
{
    internal class UnitOfWork : IUnitOfWork
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

        private IGlobalSearchRepository _globalSearchRepository;

        #endregion

        public UnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

            _dbContext.Connection.Open();
            _dbContext.Database.BeginTransaction();

            // Dapper configuration
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        #region IUnitOfWork Members

        public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_dbContext, _mapper);

        public IRoleClaimRepository RoleClaimRepository => _roleClaimRepository ??= new RoleClaimRepository(_dbContext, _mapper);

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_dbContext, _mapper);

        public IUserClaimRepository UserClaimRepository => _userClaimRepository ??= new UserClaimRepository(_dbContext, _mapper);

        public IUserLoginRepository UserLoginRepository => _userLoginRepository ??= new UserLoginRepository(_dbContext, _mapper);

        public IUserTokenRepository UserTokenRepository => _userTokenRepository ??= new UserTokenRepository(_dbContext, _mapper);

        public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_dbContext, _mapper);

        public IUserRefreshTokenRepository UserRefreshTokenRepository => _userRefreshTokenRepository ??= new UserRefreshTokenRepository(_dbContext, _mapper);

        public IClientRepository ClientRepository => _clientRepository ??= new ClientRepository(_dbContext, _mapper);

        public IBookRepository BookRepository => _bookRepository ??= new BookRepository(_dbContext, _mapper);

        public IBookstoreRepository BookstoreRepository => _bookstoreRepository ??= new BookstoreRepository(_dbContext, _mapper);

        public IEmailTemplateRepository EmailTemplateRepository => _emailTemplateRepository ??= new EmailTemplateRepository(_dbContext, _mapper);

        public IGlobalSearchRepository GlobalSearchRepository => _globalSearchRepository ??= new GlobalSearchRepository(_dbContext, _mapper);

        public async Task<ResponseStatus> CommitAsync()
        {
            try
            {
                // Transactions explained: https://docs.microsoft.com/en-us/ef/ef6/saving/transactions
                // If we don't start a transaction, SaveChangesAsync is implicit. Meaning, all updates will be available in the database immediately after the SaveChangesAsync call.
                // If we start a transaction, SaveChangesAsync still performs the updates, but the data is not available to other connections until a commit is called.
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();

                return ResponseStatus.Success;
            }
            catch
            {
                await _dbContext.Database.RollbackTransactionAsync();

                return ResponseStatus.Error;
            }
            finally
            {
                await _dbContext.Database.BeginTransactionAsync();
            }
        }

        #endregion
    }
}