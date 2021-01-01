using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserTokenRepository : DapperRepositoryBase, IUserTokenRepository
    {
        public UserTokenRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public Task AddAsync(IUserToken entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    INSERT INTO {UserTokenSchema.Table}(
                        {UserTokenSchema.Columns.UserId}, 
                        {UserTokenSchema.Columns.LoginProvider}, 
                        {UserTokenSchema.Columns.Name}, 
                        {UserTokenSchema.Columns.Value},
                        {UserTokenSchema.Columns.DateCreatedUtc},
                        {UserTokenSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.UserId)}, 
                        @{nameof(entity.LoginProvider)}, 
                        @{nameof(entity.Name)}, 
                        @{nameof(entity.Value)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public async Task<IEnumerable<IUserToken>> GetAsync()
        {
            return await QueryAsync<UserToken>(
                sql: $"SELECT * FROM {UserTokenSchema.Table}"
            );
        }

        public async Task<IUserToken> FindByKeyAsync(IUserTokenKey key)
        {
            return await QuerySingleOrDefaultAsync<UserToken>(
                sql: $@"
                    SELECT * FROM {UserTokenSchema.Table}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserTokenSchema.Columns.Name} = @{nameof(key.Name)}",
                param: key
            );
        }

        public Task DeleteByKeyAsync(IUserTokenKey key)
        {
            return ExecuteAsync(
                sql: $@"
                    DELETE FROM {UserTokenSchema.Table}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserTokenSchema.Columns.Name} = @{nameof(key.Name)}",
                param: key
            );
        }

        public Task UpdateAsync(IUserToken entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    UPDATE {UserTokenSchema.Table} SET 
                        {UserTokenSchema.Columns.Value} = @{nameof(entity.Value)},
                        {UserTokenSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(entity.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(entity.LoginProvider)} AND 
                        {UserTokenSchema.Columns.Name} = @{nameof(entity.Name)}",
                param: entity
            );
        }
    }
}