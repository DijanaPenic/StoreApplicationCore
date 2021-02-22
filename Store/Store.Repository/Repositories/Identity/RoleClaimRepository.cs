﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Helpers;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

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
    }
}
