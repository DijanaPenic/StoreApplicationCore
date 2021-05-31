using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserLoginRepository : DapperRepositoryBase, IUserLoginRepository
    {
        public UserLoginRepository(ApplicationDbContext dbContext) : base(dbContext)
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
                        {UserLoginSchema.Columns.Token}, 
                        {UserLoginSchema.Columns.IsConfirmed}, 
                        {UserLoginSchema.Columns.UserId},
                        {UserLoginSchema.Columns.DateCreatedUtc},
                        {UserLoginSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.LoginProvider)}, 
                        @{nameof(entity.ProviderKey)}, 
                        @{nameof(entity.ProviderDisplayName)}, 
                        @{nameof(entity.Token)}, 
                        @{nameof(entity.IsConfirmed)}, 
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

        public async Task<IUserLogin> FindAsync(IUserLoginKey key, bool isConfirmed)
        {
            return await QuerySingleOrDefaultAsync<UserLogin>(
                sql: $@"
                    SELECT * FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(key.ProviderKey)} AND
                        {UserLoginSchema.Columns.IsConfirmed} = {(isConfirmed ? "TRUE": "FALSE")}",       
                param: key // NOTE: key must be sent as raw param; it won't work as dynamic object -> param: new { key }
            );
        }

        public async Task<IUserLogin> FindAsync(Guid userId, string token)
        {
            return await QuerySingleOrDefaultAsync<UserLogin>(
                sql: $@"
                    SELECT * FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.UserId} = @{nameof(userId)} AND 
                        {UserLoginSchema.Columns.Token} = @{nameof(token)}",
                param: new { userId, token }
            );
        }

        public async Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId)
        {
            return await QueryAsync<UserLogin>(
                sql: $"SELECT * FROM {UserLoginSchema.Table} WHERE {UserLoginSchema.Columns.UserId} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public async Task<IEnumerable<IUserLogin>> FindByUserIdAsync(Guid userId, bool isConfirmed)
        {
            return await QueryAsync<UserLogin>(
                sql: $@"SELECT * FROM {UserLoginSchema.Table} 
                        WHERE 
                            {UserLoginSchema.Columns.UserId} = @{nameof(userId)} AND
                            {UserLoginSchema.Columns.IsConfirmed} = {(isConfirmed ? "TRUE" : "FALSE")}",
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
                        {UserLoginSchema.Columns.Token} = @{nameof(entity.Token)},
                        {UserLoginSchema.Columns.IsConfirmed} = @{nameof(entity.IsConfirmed)},
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