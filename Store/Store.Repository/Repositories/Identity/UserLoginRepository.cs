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
    internal class UserLoginRepository : DapperRepositoryBase, IUserLoginRepository
    {
        public UserLoginRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IUserLogin entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO UserLogin(
                        {nameof(UserLoginEntity.LoginProvider)}, 
                        {nameof(UserLoginEntity.ProviderKey)},
                        {nameof(UserLoginEntity.ProviderDisplayName)}, 
                        {nameof(UserLoginEntity.UserId)},
                        {nameof(UserLoginEntity.DateCreatedUtc)},
                        {nameof(UserLoginEntity.DateUpdatedUtc)})
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

        public IEnumerable<IUserLogin> Get()
        {
            return Query<UserLogin>(
                sql: $"SELECT * FROM UserLogin"
            );
        }

        public IUserLogin FindByKey(IUserLoginKey key)
        {
            return QuerySingleOrDefault<UserLogin>(
                sql: $@"
                    SELECT * FROM UserLogin
                    WHERE 
                        {nameof(UserLoginEntity.LoginProvider)} = @{nameof(key.LoginProvider)} AND 
                        {nameof(UserLoginEntity.ProviderKey)} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public IEnumerable<IUserLogin> FindByUserId(Guid userId)
        {
            return Query<UserLogin>(
                sql: $"SELECT * FROM UserLogin WHERE {nameof(UserLoginEntity.UserId)} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public void DeleteByKey(IUserLoginKey key)
        {
            Execute(
                sql: $@"
                    DELETE FROM UserLogin
                    WHERE 
                        {nameof(UserLoginEntity.LoginProvider)} = @{nameof(key.LoginProvider)} AND 
                        {nameof(UserLoginEntity.ProviderKey)} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public void Update(IUserLogin entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE UserLogin SET 
                        {nameof(UserLoginEntity.ProviderDisplayName)} = @{nameof(entity.ProviderDisplayName)},
                        {nameof(UserLoginEntity.UserId)} = @{nameof(entity.UserId)},
                        {nameof(UserLoginEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}
                    WHERE 
                        {nameof(UserLoginEntity.LoginProvider)} = @{nameof(entity.LoginProvider)} AND 
                        {nameof(UserLoginEntity.ProviderKey)} = @{nameof(entity.ProviderKey)}",
                param: entity
            );
        }
    }
}