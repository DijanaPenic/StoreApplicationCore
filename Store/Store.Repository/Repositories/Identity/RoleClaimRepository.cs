using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LinqKit;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Common.Enums;
using Store.Common.Helpers;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Entities.Identity;

namespace Store.Repositories.Identity
{
    internal class RoleClaimRepository : GenericRepository, IRoleClaimRepository
    {
        private DbSet<RoleClaimEntity> _dbSet => DbContext.Set<RoleClaimEntity>();

        public RoleClaimRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task<IPagedList<IRoleClaim>> FindAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options = null)
        {
            ExpressionStarter<IRoleClaim> predicate = PredicateBuilder.New<IRoleClaim>(rc => rc.ClaimType == filter.Type);

            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                predicate.And(rc => rc.ClaimValue.Contains(filter.SearchString));
            }
            if (!GuidHelper.IsNullOrEmpty(filter.RoleId))
            {
                predicate.And(rc => rc.RoleId == filter.RoleId);
            }

            return FindAsync<IRoleClaim, RoleClaimEntity>(predicate, paging, sorting, options);
        }

        public Task<IEnumerable<IRoleClaim>> GetAsync(ISortingParameters sorting, IOptionsParameters options = null)
        {
            return GetAsync<IRoleClaim, RoleClaimEntity>(sorting, options);
        }

        public Task<IRoleClaim> FindByKeyAsync(Guid key, IOptionsParameters options = null)
        {
            return FindByKeyAsync<IRoleClaim, RoleClaimEntity>(options, key);
        }

        public Task<ResponseStatus> UpdateAsync(IRoleClaim model)
        {
            return UpdateAsync<IRoleClaim, RoleClaimEntity>(model, model.Id);
        }

        public Task<ResponseStatus> AddAsync(IRoleClaim model)
        {
            return AddAsync<IRoleClaim, RoleClaimEntity>(model);
        }

        public Task<ResponseStatus> DeleteByKeyAsync(Guid key)
        {
            return DeleteByKeyAsync<IRoleClaim, RoleClaimEntity>(key);
        }

        public async Task<IEnumerable<IRoleClaim>> FindByRoleIdAsync(Guid roleId)
        {
            List<RoleClaimEntity> entities = await _dbSet.Where(rc => rc.RoleId == roleId).ToListAsync();

            return Mapper.Map<IEnumerable<IRoleClaim>>(entities);
        }

        public Task DeleteAsync(IRoleClaimFilteringParameters filter)
        {
            return ExecuteQueryAsync(
                sql: @$"DELETE FROM {RoleClaimSchema.Table} WHERE 
                            {RoleClaimSchema.Columns.RoleId} = @{nameof(filter.RoleId)} AND
                            {RoleClaimSchema.Columns.ClaimType} = @{nameof(filter.Type)} AND
                            {RoleClaimSchema.Columns.ClaimValue} LIKE @{nameof(filter.SearchString)}",
                param: new { filter.Type, filter.RoleId, searchString = $"%{filter.SearchString}%" }   
            );
        }
    }
}
