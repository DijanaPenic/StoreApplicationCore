using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserRefreshTokenRepository : GenericRepository, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        { 
        }

        public Task AddAsync(IUserRefreshToken entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    INSERT INTO {UserRefreshTokenSchema.Table}(
                        {UserRefreshTokenSchema.Columns.Value}, 
                        {UserRefreshTokenSchema.Columns.UserId}, 
                        {UserRefreshTokenSchema.Columns.ClientId}, 
                        {UserRefreshTokenSchema.Columns.DateExpiresUtc},
                        {UserRefreshTokenSchema.Columns.DateCreatedUtc},
                        {UserRefreshTokenSchema.Columns.DateUpdatedUtc})
                    VALUES(s
                        @{nameof(entity.Value)}, 
                        @{nameof(entity.UserId)}, 
                        @{nameof(entity.ClientId)}, 
                        @{nameof(entity.DateExpiresUtc)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public async Task<IEnumerable<IUserRefreshToken>> GetAsync()
        {
            return await QueryAsync<UserRefreshToken>(
                sql: $"SELECT * FROM {UserRefreshTokenSchema.Table}"
            );
        }

        public async Task<IUserRefreshToken> FindByKeyAsync(IUserRefreshTokenKey key)
        {
            return await QuerySingleOrDefaultAsync<UserRefreshToken>(
                            sql: $"SELECT * FROM {UserRefreshTokenSchema.Table} WHERE {UserRefreshTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND {UserRefreshTokenSchema.Columns.ClientId} = @{nameof(key.ClientId)}",
                            param: new { key }
                         );
        }

        public async Task<IUserRefreshToken> FindByValueAsync(string value)
        {
            return await QuerySingleOrDefaultAsync<UserRefreshToken>(
                            sql: $"SELECT * FROM {UserRefreshTokenSchema.Table} WHERE {UserRefreshTokenSchema.Columns.Value} = @{nameof(value)}",
                            param: new { value }
                         );
        }

        public Task DeleteByKeyAsync(IUserRefreshTokenKey key)
        {
            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserRefreshTokenSchema.Table}
                    WHERE 
                        {UserRefreshTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND
                        {UserRefreshTokenSchema.Columns.ClientId} = @{nameof(key.ClientId)}",
                param: new { key }
            );
        }

        public Task DeleteExpiredAsync()
        {
            DateTime now = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserRefreshTokenSchema.Table}
                    WHERE {UserRefreshTokenSchema.Columns.DateExpiresUtc} < @{nameof(now)}",
                param: new { now }  // Datetime must be sent as dynamic object (instead of parameter: "The JIT compiler encountered invalid IL code or an internal limitation"). 
            );
        }

        public Task UpdateAsync(IUserRefreshToken entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    UPDATE {UserRefreshTokenSchema.Table} SET 
                        {UserRefreshTokenSchema.Columns.Value} = @{nameof(entity.Value)},
                        {UserRefreshTokenSchema.Columns.DateUpdatedUtc} = {nameof(entity.DateUpdatedUtc)}
                    WHERE {UserRefreshTokenSchema.Columns.UserId} = @{nameof(entity.ClientId)} AND {UserRefreshTokenSchema.Columns.ClientId} = @{nameof(entity.ClientId)}",
                param: entity
            );
        }
    }
}