using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.Common.Enums;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Repository.Core;
using Store.Entities.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserLoginRepository : GenericRepository, IUserLoginRepository
    {
        private DbSet<UserLoginEntity> _dbSet => DbContext.Set<UserLoginEntity>();

        public UserLoginRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IEnumerable<IUserLogin>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IUserLogin, UserLoginEntity>(sorting, options);
        }

        public Task<IPagedList<IUserLogin>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            Expression<Func<IUserLogin, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString) ? null : 
                ul => ul.LoginProvider.Contains(filter.SearchString) || 
                ul.ProviderDisplayName.Contains(filter.SearchString) || 
                ul.ProviderKey.Contains(filter.SearchString);

            return FindAsync<IUserLogin, UserLoginEntity>(null, paging, sorting, options);  
        }

        public Task<IUserLogin> FindByKeyAsync(IUserLoginKey key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IUserLogin, UserLoginEntity>(options, key.LoginProvider, key.ProviderKey);
        }

        public Task<ResponseStatus> AddAsync(IUserLogin model)
        {
            return AddAsync<IUserLogin, UserLoginEntity>(model);
        }

        public Task<ResponseStatus> UpdateAsync(IUserLogin model)
        {
            return UpdateAsync<IUserLogin, UserLoginEntity>(model, model.LoginProvider, model.ProviderKey);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(IUserLoginKey key)
        {
            return DeleteByKeyAsync<IUserLogin, UserLoginEntity>(key.LoginProvider, key.ProviderKey);
        }

        public async Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId)
        {
            IEnumerable<UserLoginEntity> entities = await _dbSet.Where(ul => ul.UserId == userId).ToListAsync();

            return Mapper.Map<IEnumerable<IUserLogin>>(entities);
        }

        public async Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId, bool isConfirmed)
        {
            IEnumerable<UserLoginEntity> entities = await _dbSet.Where(ul => ul.UserId == userId && ul.IsConfirmed == isConfirmed).ToListAsync();

            return Mapper.Map<IEnumerable<IUserLogin>>(entities);
        }

        public async Task<IUserLogin> FindByUserIdAsync(Guid userId, string token)
        {
            UserLoginEntity entities = await _dbSet.Where(ul => ul.UserId == userId && ul.Token == token).SingleOrDefaultAsync();

            return Mapper.Map<IUserLogin>(entities);
        }
    }
}