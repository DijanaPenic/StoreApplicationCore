using System;
using System.Text;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Models;
using Store.Models.Identity;
using Store.Common.Helpers;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

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

        public Task DeleteAsync(Guid roleId, string type, string searchString)
        {
            return ExecuteAsync(
                sql: @$"DELETE FROM {RoleClaimSchema.Table} WHERE 
                            {RoleClaimSchema.Columns.RoleId} = @{nameof(roleId)} AND
                            {RoleClaimSchema.Columns.ClaimType} = @{nameof(type)} AND
                            {RoleClaimSchema.Columns.ClaimValue} LIKE @{nameof(searchString)}",
                param: new { type, searchString = $"%{searchString}%" }   
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

        public async Task<IPagedEnumerable<IRoleClaim>> FindAsync(string type, string searchString, bool isDescendingSortOrder, int pageNumber, int pageSize, Guid? roleId = null)
        {
            IList<string> filterConditions = new List<string>
            {
                $"rc.{ RoleClaimSchema.Columns.ClaimType} = @{nameof(type)}"
            };
            if (searchString != null) filterConditions.Add(@$"LOWER(rc.{RoleClaimSchema.Columns.ClaimValue}) LIKE @{nameof(searchString)}");
            if (!GuidHelper.IsNullOrEmpty(roleId)) filterConditions.Add($"rc.{RoleClaimSchema.Columns.RoleId} = @{nameof(roleId)}");

            dynamic searchParameters = new ExpandoObject();
            searchParameters.Type = type;
            searchParameters.SearchString = $"%{searchString?.ToLowerInvariant()}%";
            searchParameters.RoleId = roleId;

            using GridReader reader = await FindAsync
            (
                table: $"{RoleClaimSchema.Table} rc",
                select: "*",
                filter: new StringBuilder("WHERE ").AppendJoin(" AND ", filterConditions).ToString(),
                include: string.Empty,
                sortOrderProperty: $"rc.{RoleClaimSchema.Columns.ClaimValue}",
                isDescendingSortOrder: isDescendingSortOrder,
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchParameters: searchParameters
            );

            IEnumerable<IRoleClaim> roleClaims = reader.Read<RoleClaim>();
            int totalCount = reader.ReadFirst<int>();

            return new PagedEnumerable<IRoleClaim>(roleClaims, totalCount, pageSize, pageNumber);
        }
    }
}
