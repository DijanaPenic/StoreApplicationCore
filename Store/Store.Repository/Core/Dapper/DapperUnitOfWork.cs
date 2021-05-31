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

        public DapperUnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

            _dbContext.Connection.Open();
            _dbContext.Database.BeginTransaction();

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

        public async Task CommitAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();
            }
            catch
            {
                await _dbContext.Database.RollbackTransactionAsync();
            }
            finally
            {
                await _dbContext.Database.BeginTransactionAsync();
            }
        }

        #endregion
    }
}