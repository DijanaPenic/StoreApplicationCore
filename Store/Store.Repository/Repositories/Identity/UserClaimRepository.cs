using System;
using System.Data;
using System.Collections.Generic;

using Store.Entities.Identity;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

namespace Store.Repository.Repositories.Identity
{
    internal class UserClaimRepository : DapperRepositoryBase, IUserClaimRepository
    {
        public UserClaimRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public void Add(IUserClaim entity)
        {
            entity.Id = ExecuteScalar<Guid>(
                sql: $@"
                    INSERT INTO UserClaim(
                        {nameof(UserClaimEntity.ClaimType)}, 
                        {nameof(UserClaimEntity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)})
                    VALUES(
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.UserId)});
                    SELECT SCOPE_IDENTITY()",
                param: entity
            );
        }

        public IEnumerable<IUserClaim> Get()
        {
            return Query<UserClaim>(
                sql: $@"
                    SELECT {nameof(UserClaimEntity.Id)}, {nameof(UserClaimEntity.ClaimType)}, {nameof(UserClaimEntity.ClaimValue)}, {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim"
            );
        }

        public IUserClaim FindByKey(Guid key)
        {
            return QuerySingleOrDefault<UserClaim>(
                sql: $@"
                    SELECT {nameof(UserClaimEntity.Id)}, {nameof(UserClaimEntity.ClaimType)}, {nameof(UserClaimEntity.ClaimValue)}, {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim WHERE {nameof(UserClaimEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IEnumerable<IUserClaim> GetByUserId(string userId)
        {
            return Query<UserClaim>(
                sql: $@"
                    SELECT {nameof(UserClaimEntity.Id)}, {nameof(UserClaimEntity.ClaimType)}, {nameof(UserClaimEntity.ClaimValue)}, {nameof(UserClaimEntity.UserId)}
                    FROM UserClaim
                    WHERE {nameof(UserClaimEntity.UserId)} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public IEnumerable<IUser> GetUsersForClaim(string claimType, string claimValue)
        {
            return Query<User>(
                sql: $@"
                    SELECT
	                    u.{nameof(IUser.Id)}, u.{nameof(IUser.AccessFailedCount)}, u.{nameof(IUser.ConcurrencyStamp)}, u.{nameof(IUser.Email)},
                        u.{nameof(IUser.EmailConfirmed)}, u.{nameof(IUser.LockoutEnabled)}, u.{nameof(IUser.LockoutEndDateUtc)},
                        u.{nameof(IUser.NormalizedEmail)}, u.{nameof(IUser.NormalizedUserName)}, u.{nameof(IUser.PasswordHash)},
                        u.{nameof(IUser.PhoneNumber)}, u.{nameof(IUser.PhoneNumberConfirmed)}, u.{nameof(IUser.SecurityStamp)},
	                    u.{nameof(IUser.TwoFactorEnabled)}, u.{nameof(IUser.UserName)}
                    FROM
	                    UserClaim c INNER JOIN
                        User u ON c.{nameof(IUserClaim.UserId)} = u.{nameof(IUser.Id)}
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
            Execute(
                sql: $@"
                    UPDATE UserClaim SET 
                        {nameof(UserClaimEntity.ClaimType)} = @{nameof(entity.ClaimType)},
                        {nameof(UserClaimEntity.ClaimValue)} = @{nameof(entity.ClaimValue)}, 
                        {nameof(UserClaimEntity.UserId)} = @{nameof(entity.UserId)}
                    WHERE Id = @Id",
                param: entity
            );
        }
    }
}