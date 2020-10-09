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
    internal class UserRepository : DapperRepositoryBase, IUserRepository
    {
        public UserRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IUser entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO User(
                        {nameof(UserEntity.Id)}, 
                        {nameof(UserEntity.FirstName)}, 
                        {nameof(UserEntity.LastName)}, 
                        {nameof(UserEntity.AccessFailedCount)}, 
                        {nameof(UserEntity.ConcurrencyStamp)}, 
                        {nameof(UserEntity.Email)}, 
                        {nameof(UserEntity.EmailConfirmed)},
	                    {nameof(UserEntity.LockoutEnabled)}, 
                        {nameof(UserEntity.LockoutEndDateUtc)}, 
                        {nameof(UserEntity.NormalizedEmail)}, 
                        {nameof(UserEntity.NormalizedUserName)},
	                    {nameof(UserEntity.PasswordHash)}, 
                        {nameof(UserEntity.PhoneNumber)}, 
                        {nameof(UserEntity.PhoneNumberConfirmed)}, 
                        {nameof(UserEntity.SecurityStamp)},
	                    {nameof(UserEntity.TwoFactorEnabled)}, 
	                    {nameof(UserEntity.IsApproved)}, 
	                    {nameof(UserEntity.IsDeleted)}, 
                        {nameof(UserEntity.UserName)},
                        {nameof(UserEntity.DateCreatedUtc)},
                        {nameof(UserEntity.DateUpdatedUtc)})
                    VALUES(
                        @{nameof(entity.Id)}, 
                        @{nameof(entity.FirstName)}, 
                        @{nameof(entity.LastName)}, 
                        @{nameof(entity.AccessFailedCount)}, 
                        @{nameof(entity.ConcurrencyStamp)}, 
                        @{nameof(entity.Email)}, 
                        @{nameof(entity.EmailConfirmed)},
	                    @{nameof(entity.LockoutEnabled)}, 
                        @{nameof(entity.LockoutEndDateUtc)}, 
                        @{nameof(entity.NormalizedEmail)}, 
                        @{nameof(entity.NormalizedUserName)},
	                    @{nameof(entity.PasswordHash)}, 
                        @{nameof(entity.PhoneNumber)}, 
                        @{nameof(entity.PhoneNumberConfirmed)}, 
                        @{nameof(entity.SecurityStamp)},
	                    @{nameof(entity.TwoFactorEnabled)}, 
	                    @{nameof(entity.IsApproved)},
	                    @{nameof(entity.IsDeleted)},
                        @{nameof(entity.UserName)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public IEnumerable<IUser> Get()
        {
            return Query<User>(
                sql: $"SELECT * FROM User"
            );
        }

        public IUser FindByKey(Guid key)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM User WHERE {nameof(UserEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IUser FindByNormalizedEmail(string normalizedEmail)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM User WHERE {nameof(UserEntity.NormalizedEmail)} = @{nameof(normalizedEmail)}",
                param: new { normalizedEmail }
            );
        }

        public IUser FindByNormalizedUserName(string normalizedUserName)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM User WHERE {nameof(UserEntity.NormalizedUserName)} = @{nameof(normalizedUserName)}",
                param: new { normalizedUserName }
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM User WHERE {nameof(UserEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IUser entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE User SET 
                        {nameof(UserEntity.FirstName)} = @{nameof(entity.FirstName)},
                        {nameof(UserEntity.LastName)} = @{nameof(entity.LastName)},
                        {nameof(UserEntity.AccessFailedCount)} = @{nameof(entity.AccessFailedCount)},
	                    {nameof(UserEntity.ConcurrencyStamp)} = @{nameof(entity.ConcurrencyStamp)}, 
                        {nameof(UserEntity.Email)} = @{nameof(entity.Email)},
	                    {nameof(UserEntity.EmailConfirmed)} = @{nameof(entity.EmailConfirmed)}, 
                        {nameof(UserEntity.LockoutEnabled)} = @{nameof(entity.LockoutEnabled)},
	                    {nameof(UserEntity.LockoutEndDateUtc)} = @{nameof(entity.LockoutEndDateUtc)}, 
                        {nameof(UserEntity.NormalizedEmail)} = @{nameof(entity.NormalizedEmail)},
	                    {nameof(UserEntity.NormalizedUserName)} = @{nameof(entity.NormalizedUserName)}, 
                        {nameof(UserEntity.PasswordHash)} = @{nameof(entity.PasswordHash)},
	                    {nameof(UserEntity.PhoneNumber)} = @{nameof(entity.PhoneNumber)}, 
                        {nameof(UserEntity.PhoneNumberConfirmed)} = @{nameof(entity.PhoneNumberConfirmed)},
	                    {nameof(UserEntity.SecurityStamp)} = @{nameof(entity.SecurityStamp)}, 
                        {nameof(UserEntity.TwoFactorEnabled)} = @{nameof(entity.TwoFactorEnabled)},
                        {nameof(UserEntity.IsApproved)} = @{nameof(entity.IsApproved)},
                        {nameof(UserEntity.IsDeleted)} = @{nameof(entity.IsDeleted)},
	                    {nameof(UserEntity.UserName)} = @{nameof(entity.UserName)},
                        {nameof(RoleEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}

                    WHERE {nameof(UserEntity.Id)} = @{nameof(entity.Id)}",
                param: entity);
        }
    }
}