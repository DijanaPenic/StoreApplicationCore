using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
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
using Store.Repository.Common.Repositories.Identity;
using Store.Entities.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repositories.Identity
{
    internal class UserRepository : GenericRepository, IUserRepository
    {
        private DbSet<UserEntity> _dbSet => DbContext.Set<UserEntity>();

        public UserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IPagedList<IUser>> FindAsync(IUserFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IUser> predicate = PredicateBuilder.New<IUser>(true);

            if (!filter.ShowInactive)
            {
                predicate.And(u => u.IsApproved == true);
            }
            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(u => u.FirstName.Contains(filter.SearchString) || u.LastName.Contains(filter.SearchString));
            }

            return FindAsync<IUser, UserEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IUser>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IUser, UserEntity>(sorting, options);
        }

        public Task<IUser> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IUser, UserEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IUser model)
        {
            return UpdateAsync<IUser, UserEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IUser model)
        {
            return AddAsync<IUser, UserEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<UserEntity>(key);
        }


        public async Task<IUser> FindByNormalizedEmailAsync(string normalizedEmail)
        {
            UserEntity entity = await _dbSet.Where(r => r.NormalizedEmail == normalizedEmail).SingleOrDefaultAsync();

            return Mapper.Map<IUser>(entity);
        }

        public async Task<IUser> FindByNormalizedUserNameAsync(string normalizedUserName)
        {
            UserEntity entity = await _dbSet.Where(r => r.NormalizedUserName == normalizedUserName).SingleOrDefaultAsync();

            return Mapper.Map<IUser>(entity);
        }
    }
}