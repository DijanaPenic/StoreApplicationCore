using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Common.Helpers;
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
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteQueryAsync(
                sql: $@"
                    INSERT INTO {UserRefreshTokenSchema.Table}(
                        {UserRefreshTokenSchema.Columns.Id},
                        {UserRefreshTokenSchema.Columns.Value}, 
                        {UserRefreshTokenSchema.Columns.UserId}, 
                        {UserRefreshTokenSchema.Columns.ClientId}, 
                        {UserRefreshTokenSchema.Columns.DateExpiresUtc},
                        {UserRefreshTokenSchema.Columns.DateCreatedUtc},
                        {UserRefreshTokenSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.Id)}, 
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

        public async Task<IUserRefreshToken> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<UserRefreshToken>(
                            sql: $"SELECT * FROM {UserRefreshTokenSchema.Table} WHERE {UserRefreshTokenSchema.Columns.Id} = @{nameof(key)}",
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

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserRefreshTokenSchema.Table}
                    WHERE {UserRefreshTokenSchema.Columns.Id} = @{nameof(key)}",
                param: key
            );
        }

        public Task DeleteAsync(Guid userId, Guid clientId)
        {
            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserRefreshTokenSchema.Table}
                    WHERE 
                        {UserRefreshTokenSchema.Columns.UserId} = @{nameof(userId)} AND
                        {UserRefreshTokenSchema.Columns.ClientId} = @{nameof(clientId)}",
                param: new { userId, clientId }
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
                    WHERE {UserRefreshTokenSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}