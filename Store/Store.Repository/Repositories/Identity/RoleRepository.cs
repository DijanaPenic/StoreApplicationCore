using System;
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
    internal class RoleRepository : DapperRepositoryBase, IRoleRepository
    {
        public RoleRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public Task AddAsync(IRole entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteAsync(
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
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Name} = @{nameof(roleName)}",
                param: new { roleName }
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteAsync(
                sql: $"DELETE FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task UpdateAsync(IRole entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
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