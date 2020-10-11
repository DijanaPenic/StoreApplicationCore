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
                    INSERT INTO {UserLoginSchema.Table}(
                        {UserLoginSchema.Columns.LoginProvider}, 
                        {UserLoginSchema.Columns.ProviderKey},
                        {UserLoginSchema.Columns.ProviderDisplayName}, 
                        {UserLoginSchema.Columns.UserId},
                        {UserLoginSchema.Columns.DateCreatedUtc},
                        {UserLoginSchema.Columns.DateUpdatedUtc})
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
                sql: $"SELECT * FROM {UserLoginSchema.Table}"
            );
        }

        public IUserLogin FindByKey(IUserLoginKey key)
        {
            return QuerySingleOrDefault<UserLogin>(
                sql: $@"
                    SELECT * FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public IEnumerable<IUserLogin> FindByUserId(Guid userId)
        {
            return Query<UserLogin>(
                sql: $"SELECT * FROM {UserLoginSchema.Table} WHERE {UserLoginSchema.Columns.UserId} = @{nameof(userId)}",
                param: new { userId }
            );
        }

        public void DeleteByKey(IUserLoginKey key)
        {
            Execute(
                sql: $@"
                    DELETE FROM {UserLoginSchema.Table}
                    WHERE 
                        {UserLoginSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        {UserLoginSchema.Columns.ProviderKey} = @{nameof(key.ProviderKey)}",
                param: key
            );
        }

        public void Update(IUserLogin entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE {UserLoginSchema.Table} SET 
                        {UserLoginSchema.Columns.ProviderDisplayName} = @{nameof(entity.ProviderDisplayName)},
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