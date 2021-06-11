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
    internal class UserClaimRepository : GenericRepository, IUserClaimRepository
    {
        private DbSet<UserClaimEntity> _dbSet => DbContext.Set<UserClaimEntity>();

        public UserClaimRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task<IPagedList<IUserClaim>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IUserClaim> predicate = PredicateBuilder.New<IUserClaim>();

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(uc => uc.ClaimValue.Contains(filter.SearchString));
            }

            return FindAsync<IUserClaim, UserClaimEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IUserClaim>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IUserClaim, UserClaimEntity>(sorting, options);
        }

        public Task<IUserClaim> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IUserClaim, UserClaimEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IUserClaim model)
        {
            return UpdateAsync<IUserClaim, UserClaimEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IUserClaim model)
        {
            return AddAsync<IUserClaim, UserClaimEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IUserClaim, UserClaimEntity>(key);
        }

        public async Task<IEnumerable<IUserClaim>> FindByUserIdAsync(Guid userId)
        {
            List<UserClaimEntity> entities = await _dbSet.Where(uc => uc.UserId == userId).ToListAsync();

            return Mapper.Map<IEnumerable<IUserClaim>>(entities);
        }

        public async Task<IEnumerable<IUser>> FindUsersByClaimAsync(string claimType, string claimValue)
        {
            var entities = await _dbSet.Where(uc => uc.ClaimType == claimType && uc.ClaimValue == claimValue).Select(uc => uc.User).ToListAsync();

            return Mapper.Map<IEnumerable<IUser>>(entities);
        }
    }
}