using System;
using System.Data;
using System.Collections.Generic;

using Store.Entities.Identity;
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
                    INSERT INTO UserClaim(
                        {nameof(UserClaimEntity.ClaimType)}, 
                        {nameof(UserClaimEntity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)},
                        {nameof(UserClaimEntity.DateCreatedUtc)},
                        {nameof(UserClaimEntity.DateUpdatedUtc)})
                    VALUES(
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.UserId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});
                    RETURNING {nameof(UserClaimEntity.Id)};",
                param: entity
            );
        }

        public IEnumerable<IUserClaim> Get()
        {
            return Query<UserClaim>(
                sql: $@"
                    SELECT 
                        {nameof(UserClaimEntity.Id)}, 
                        {nameof(UserClaimEntity.ClaimType)}, 
                        {nameof(UserClaimEntity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim"
            );
        }

        public IUserClaim FindByKey(Guid key)
        {
            return QuerySingleOrDefault<UserClaim>(
                sql: $@"
                    SELECT 
                        {nameof(UserClaimEntity.Id)}, 
                        {nameof(UserClaimEntity.ClaimType)}, 
                        {nameof(UserClaimEntity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim WHERE {nameof(UserClaimEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IEnumerable<IUserClaim> GetByUserId(Guid userId)
        {
            return Query<UserClaim>(
                sql: $@"
                    SELECT 
                        {nameof(UserClaimEntity.Id)}, 
                        {nameof(UserClaimEntity.ClaimType)}, 
                        {nameof(UserClaimEntity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim WHERE {nameof(UserClaimEntity.UserId)} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public IEnumerable<IUser> GetUsersForClaim(string claimType, string claimValue)
        {
            return Query<User>(
                sql: $@"
                    SELECT
	                    u.{nameof(UserEntity.Id)}, 
                        u.{nameof(UserEntity.AccessFailedCount)}, 
                        u.{nameof(UserEntity.ConcurrencyStamp)}, 
                        u.{nameof(UserEntity.Email)},
                        u.{nameof(UserEntity.EmailConfirmed)}, 
                        u.{nameof(UserEntity.LockoutEnabled)}, 
                        u.{nameof(UserEntity.LockoutEndDateUtc)},
                        u.{nameof(UserEntity.NormalizedEmail)}, 
                        u.{nameof(UserEntity.NormalizedUserName)}, 
                        u.{nameof(UserEntity.PasswordHash)},
                        u.{nameof(UserEntity.PhoneNumber)}, 
                        u.{nameof(UserEntity.PhoneNumberConfirmed)}, 
                        u.{nameof(UserEntity.SecurityStamp)},
	                    u.{nameof(UserEntity.TwoFactorEnabled)}, 
                        u.{nameof(UserEntity.UserName)}
                    FROM UserClaim c 
                        INNER JOIN User u ON c.{nameof(UserClaimEntity.UserId)} = u.{nameof(UserEntity.Id)}
                    WHERE
	                    c.{nameof(UserClaimEntity.ClaimType)} = @{nameof(claimType)} AND
                        c.{nameof(UserClaimEntity.ClaimValue)} = @{nameof(claimValue)}
                ",
                param: new { claimType, claimValue }
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $@"
                    DELETE FROM UserClaim
                    WHERE {nameof(UserClaimEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IUserClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE UserClaim SET 
                        {nameof(UserClaimEntity.ClaimType)} = @{nameof(entity.ClaimType)},
                        {nameof(UserClaimEntity.ClaimValue)} = @{nameof(entity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)} = @{nameof(entity.UserId)},
                        {nameof(UserClaimEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}
                    WHERE {nameof(UserClaimEntity.Id)} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}