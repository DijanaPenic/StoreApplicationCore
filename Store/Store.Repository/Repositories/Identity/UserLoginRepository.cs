using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.DAL.Schema;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserLoginRepository : DapperRepositoryBase, IUserLoginRepository
    {
        public UserLoginRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public Task AddAsync(IUserLogin entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    INSERT INTO {UserLoginSchema.Table}(
                        {UserLoginSchema.Columns.LoginProvider}, 
                        {UserLoginSchema.Columns.ProviderKey},
                        {UserLoginSchema.Columns.ProviderDisplayName}, 
                        {UserLoginSchema.Columns.UserId},
                        {UserLoginSchema.Columns.DateCreatedUtc},
                        {UserLoginSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.LoginProvider)}, 
                        @{nameof(entity.ProviderKey)}, 
                        @{nameof(entity.ProviderDisplayName)}, 
                        @{nameof(entity.UserId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public async Task<IEnumerable<IUserLogin>> GetAsync()
        {
            return await QueryAsync<UserLogin>(
                sql: $"SELECT * FROM {UserLoginSchema.Table}"
            );
        }

        public async Task<IUserLogin> FindByKeyAsync(IUserLoginKey key)
        {
            return await QuerySingleOrDefaultAsync<UserLogin>(
                sql: $@"
                    SELECT * FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public async Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId)
        {
            return await QueryAsync<UserLogin>(
                sql: $"SELECT * FROM {UserLoginSchema.Table} WHERE {UserLoginSchema.Columns.UserId} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public Task DeleteByKeyAsync(IUserLoginKey key)
        {
            return ExecuteAsync(
                sql: $@"
                    DELETE FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public Task UpdateAsync(IUserLogin entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    UPDATE {UserLoginSchema.Table} SET 
                        {UserLoginSchema.Columns.ProviderDisplayName} = @{nameof(entity.ProviderDisplayName)},
                        {UserLoginSchema.Columns.UserId} = @{nameof(entity.UserId)},
                        {UserLoginSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(entity.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(entity.ProviderKey)}",
                param: entity
            );
        }
    }
}