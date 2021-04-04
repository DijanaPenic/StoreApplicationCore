using System;
using System.Text;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Models;
using Store.Models.Identity;
using Store.Common.Helpers;
using Store.Common.Parameters.Paging;
using Store.Common.Parameters.Sorting;
using Store.Common.Parameters.Filtering;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Models;
using Store.Repository.Common.Repositories.Identity;
using Store.Repository.Repositories.Models;

using static Dapper.SqlMapper;

namespace Store.Repositories.Identity
{
    internal class RoleClaimRepository : DapperRepositoryBase, IRoleClaimRepository
    {
        public RoleClaimRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public Task AddAsync(IRoleClaim entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteAsync(
                sql: $@"
                    INSERT INTO {RoleClaimSchema.Table}(
                        {RoleClaimSchema.Columns.Id},
                        {RoleClaimSchema.Columns.ClaimType}, 
                        {RoleClaimSchema.Columns.ClaimValue}, 
                        {RoleClaimSchema.Columns.RoleId},
                        {RoleClaimSchema.Columns.DateCreatedUtc},
                        {RoleClaimSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.Id)},
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.RoleId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});",
                param: entity
            );
        }

        public async Task<IRoleClaim> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IEnumerable<IRoleClaim>> FindByRoleIdAsync(Guid roleId)
        {
            return await QueryAsync<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.RoleId} = @{nameof(roleId)}",
                param: new { roleId }
            );
        }

        public async Task<IEnumerable<IRoleClaim>> GetAsync()
        {
            return await QueryAsync<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table}"
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteAsync(
                sql: $"DELETE FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task DeleteAsync(IRoleClaimFilteringParameters filter)
        {
            return ExecuteAsync(
                sql: @$"DELETE FROM {RoleClaimSchema.Table} WHERE 
                            {RoleClaimSchema.Columns.RoleId} = @{nameof(filter.RoleId)} AND
                            {RoleClaimSchema.Columns.ClaimType} = @{nameof(filter.Type)} AND
                            {RoleClaimSchema.Columns.ClaimValue} LIKE @{nameof(filter.SearchString)}",
                param: new { filter.Type, filter.RoleId, searchString = $"%{filter.SearchString}%" }   
            );
        }

        public Task UpdateAsync(IRoleClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    UPDATE {RoleClaimSchema.Table} SET 
                        {RoleClaimSchema.Columns.ClaimType} = @{nameof(entity.ClaimType)}, 
                        {RoleClaimSchema.Columns.ClaimValue} = @{nameof(entity.ClaimType)}, 
                        {RoleClaimSchema.Columns.RoleId} = @{nameof(entity.ClaimType)},
                        {RoleClaimSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE {RoleClaimSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }

        public async Task<IPagedEnumerable<IRoleClaim>> FindAsync(IRoleClaimFilteringParameters filter, IPagingParameters paging, ISortingParameters sorting)
        {
            IList<string> filterConditions = new List<string>
            {
                $"rc.{ RoleClaimSchema.Columns.ClaimType} = @{nameof(filter.Type)}"
            };
            if (filter.SearchString != null) filterConditions.Add(@$"LOWER(rc.{RoleClaimSchema.Columns.ClaimValue}) LIKE @{nameof(filter.SearchString)}");
            if (!GuidHelper.IsNullOrEmpty(filter.RoleId)) filterConditions.Add($"rc.{RoleClaimSchema.Columns.RoleId} = @{nameof(filter.RoleId)}");

            dynamic searchParameters = new ExpandoObject();
            searchParameters.Type = filter.Type;
            searchParameters.SearchString = $"%{filter.SearchString?.ToLowerInvariant()}%";
            searchParameters.RoleId = filter.RoleId;

            IFilterModel filterModel = new FilterModel()
            {
                Expression = new StringBuilder("WHERE ").AppendJoin(" AND ", filterConditions).ToString(),
                Parameters = searchParameters
            };

            IQueryTableModel queryTableModel = new QueryTableModel()
            {
                TableName = RoleClaimSchema.Table,
                TableAlias = "rc",
                SelectStatement = "*",
                IncludeStatement = string.Empty
            };

            using GridReader reader = await FindAsync(queryTableModel, filterModel, paging, sorting);

            IEnumerable<IRoleClaim> roleClaims = reader.Read<RoleClaim>();
            int totalCount = reader.ReadFirst<int>();

            return new PagedEnumerable<IRoleClaim>(roleClaims, totalCount, paging.PageSize, paging.PageNumber);
        }
    }
}
