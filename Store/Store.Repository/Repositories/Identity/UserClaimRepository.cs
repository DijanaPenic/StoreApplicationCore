using System;
using System.Data;
using System.Collections.Generic;

using Store.DAL.Schema;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repositories.Identity
{
    internal class UserClaimRepository : DapperRepositoryBase, IUserClaimRepository
    {
        public UserClaimRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public void Add(IUserClaim entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            entity.Id = ExecuteScalar<Guid>(
                sql: $@"
                    INSERT INTO {UserClaimSchema.Table}(
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId},
                        {UserClaimSchema.Columns.DateCreatedUtc},
                        {UserClaimSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.UserId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});
                    RETURNING {UserClaimSchema.Columns.Id};",
                param: entity
            );
        }

        public IEnumerable<IUserClaim> Get()
        {
            return Query<UserClaim>(
                sql: $@"
                    SELECT 
                        {UserClaimSchema.Columns.Id}, 
                        {UserClaimSchema.Columns.ClaimType}, 
                        {UserClaimSchema.Columns.ClaimValue}, 
                        {UserClaimSchema.Columns.UserId}
                    FROM {UserClaimSchema.Table}"
            );
        }

        public IUserClaim FindByKey(Guid key)
        {
            return QuerySingleOrDefault<UserClaim>(
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

        public IEnumerable<IUserClaim> GetByUserId(Guid userId)
        {
            return Query<UserClaim>(
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

        public IEnumerable<IUser> GetUsersForClaim(string claimType, string claimValue)
        {
            return Query<User>(
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

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $@"
                    DELETE FROM {UserClaimSchema.Table}
                    WHERE {UserClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IUserClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
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