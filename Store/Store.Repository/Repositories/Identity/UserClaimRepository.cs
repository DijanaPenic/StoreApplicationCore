using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Common.Helpers;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Models.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repositories.Identity
{
    internal class UserClaimRepository : GenericRepository, IUserClaimRepository
    {
        public UserClaimRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task AddAsync(IUserClaim entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteQueryAsync(
                sql: $@"
                    INSERT INTO {UserClaimSchema.Table}(
                        {UserClaimSchema.Columns.Id},
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId},
                        {UserClaimSchema.Columns.DateCreatedUtc},
                        {UserClaimSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.Id)},
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.UserId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});",
                param: entity
            );
        }

        public async Task<IEnumerable<IUserClaim>> GetAsync()
        {
            return await QueryAsync<UserClaim>(
                sql: $@"
                    SELECT 
                        {UserClaimSchema.Columns.Id}, 
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId}
                    FROM {UserClaimSchema.Table}"
            );
        }

        public async Task<IUserClaim> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<UserClaim>(
                sql: $@"
                    SELECT 
                        {UserClaimSchema.Columns.Id}, 
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId}
                    FROM {UserClaimSchema.Table} WHERE {UserClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IEnumerable<IUserClaim>> FindByUserIdAsync(Guid userId)
        {
            return await QueryAsync<UserClaim>(
                sql: $@"
                    SELECT 
                        {UserClaimSchema.Columns.Id}, 
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId}
                    FROM {UserClaimSchema.Table} WHERE {UserClaimSchema.Columns.UserId} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public async Task<IEnumerable<IUser>> FindUsersByClaimAsync(string claimType, string claimValue)
        {
            return await QueryAsync<User>(
                sql: $@"
                    SELECT
	                    u.{UserSchema.Columns.Id}, 
                        u.{UserSchema.Columns.AccessFailedCount}, 
                        u.{UserSchema.Columns.ConcurrencyStamp}, 
                        u.{UserSchema.Columns.Email},
                        u.{UserSchema.Columns.EmailConfirmed}, 
                        u.{UserSchema.Columns.LockoutEnabled}, 
                        u.{UserSchema.Columns.LockoutEndDateUtc},
                        u.{UserSchema.Columns.NormalizedEmail}, 
                        u.{UserSchema.Columns.NormalizedUserName}, 
                        u.{UserSchema.Columns.PasswordHash},
                        u.{UserSchema.Columns.PhoneNumber}, 
                        u.{UserSchema.Columns.PhoneNumberConfirmed}, 
                        u.{UserSchema.Columns.SecurityStamp},
	                    u.{UserSchema.Columns.TwoFactorEnabled}, 
                        u.{UserSchema.Columns.UserName}
                    FROM {UserClaimSchema.Table} c 
                        INNER JOIN {UserSchema.Table} u ON c.{UserClaimSchema.Columns.UserId} = u.{UserSchema.Columns.Id}
                    WHERE
	                    c.{UserClaimSchema.Columns.ClaimType} = @{nameof(claimType)} AND
                        c.{UserClaimSchema.Columns.ClaimValue} = @{nameof(claimValue)}
                ",
                param: new { claimType, claimValue }
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteQueryAsync(
                sql: $@"
                    DELETE FROM {UserClaimSchema.Table}
                    WHERE {UserClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task UpdateAsync(IUserClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteQueryAsync(
                sql: $@"
                    UPDATE {UserClaimSchema.Table} SET 
                        {UserClaimSchema.Columns.ClaimType} = @{nameof(entity.ClaimType)},
                        {UserClaimSchema.Columns.ClaimValue} = @{nameof(entity.ClaimValue)}, 
                        {UserClaimSchema.Columns.UserId} = @{nameof(entity.UserId)},
                        {UserClaimSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE {UserClaimSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}