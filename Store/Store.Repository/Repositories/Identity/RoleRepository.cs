using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using X.PagedList;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Common.Helpers;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Entities.Identity;
using Store.Common.Extensions;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Options;
using Store.Common.Parameters.Filtering;

namespace Store.Repositories.Identity
{
    internal class RoleRepository : GenericRepository, IRoleRepository
    {
        private DbSet<RoleEntity> _dbSet => DbContext.Set<RoleEntity>();

        public RoleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task AddAsync(IRole entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteQueryAsync(
                sql: $@"
                    INSERT INTO {RoleSchema.Table}(
                        {RoleSchema.Columns.Id}, 
                        {RoleSchema.Columns.ConcurrencyStamp}, 
                        {RoleSchema.Columns.Name}, 
                        {RoleSchema.Columns.NormalizedName},
                        {RoleSchema.Columns.Stackable},
                        {RoleSchema.Columns.DateCreatedUtc},
                        {RoleSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.Id)}, 
                        @{nameof(entity.ConcurrencyStamp)}, 
                        @{nameof(entity.Name)}, 
                        @{nameof(entity.NormalizedName)},
                        @{nameof(entity.Stackable)}, 
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public async Task<IEnumerable<IRole>> GetAsync()
        {
            return await QueryAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table}"
            );
        }

        public Task<IRole> FindByKeyAsync(Guid key, IOptionsParameters options)
        {
            return FindByIdAsync<IRole, RoleEntity>(key, options);
        }

        public Task<IPagedList<IRole>> FindAsync(IFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting, IOptionsParameters options)
        {
            Expression<Func<IRole, bool>> filterExpression = string.IsNullOrEmpty(filter.SearchString) ? null : r => r.Name.Contains(filter.SearchString);

            return FindAsync<IRole, RoleEntity>(filterExpression, paging, sorting, options);
        }

        public async Task<IPagedList<IRole>> FindBySectionAsync(IPermissionFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting)
        {
            string sectionType = $"{filter.SectionType}.";

            IPagedList<RoleEntity> entityPagedList = await _dbSet.Filter(string.IsNullOrEmpty(filter.SearchString) ? null : r => r.Name.Contains(filter.SearchString))
                                                                 .Filter(r => r.Claims.Any(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .Include(r => r.Claims.Where(rc => rc.ClaimValue.StartsWith(sectionType)))
                                                                 .OrderBy(SortingMap<IRole, RoleEntity>(sorting))
                                                                 .ToPagedListAsync(paging.PageNumber, paging.PageSize);

            return entityPagedList.ToPagedList<RoleEntity, IRole>(Mapper);
        }

        public async Task<IRole> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IRole> FindByNameAsync(string roleName)
        {
            return await QuerySingleOrDefaultAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.NormalizedName} = @{nameof(roleName)}",
                param: new { roleName }
            );
        }

        public async Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames)
        {
            return await QueryAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.NormalizedName} = ANY(@{nameof(roleNames)})",
                param: new { roleNames }
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteQueryAsync(
                sql: $"DELETE FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task UpdateAsync(IRole entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    UPDATE {RoleSchema.Table} SET 
                        {RoleSchema.Columns.ConcurrencyStamp} = @{nameof(entity.ConcurrencyStamp)}, 
                        {RoleSchema.Columns.Name} = @{nameof(entity.Name)}, 
                        {RoleSchema.Columns.NormalizedName} = @{nameof(entity.NormalizedName)},
                        {RoleSchema.Columns.Stackable} = @{nameof(entity.Stackable)},
                        {RoleSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE {RoleSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}