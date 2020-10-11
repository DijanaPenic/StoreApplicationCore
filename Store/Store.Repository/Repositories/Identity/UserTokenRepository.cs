using System;
using System.Data;
using System.Collections.Generic;

using Store.DAL.Schema;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repositories.Identity
{
    internal class UserTokenRepository : DapperRepositoryBase, IDapperGenericRepository<IUserToken, IUserTokenKey>
    {
        public UserTokenRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IUserToken entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO {UserTokenSchema.Table}(
                        {UserTokenSchema.Columns.UserId}, 
                        {UserTokenSchema.Columns.LoginProvider}, 
                        [{UserTokenSchema.Columns.Name}], 
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

        public IEnumerable<IUserToken> Get()
        {
            return Query<UserToken>(
                sql: $"SELECT * FROM {UserTokenSchema.Table}"
            );
        }

        public IUserToken FindByKey(IUserTokenKey key)
        {
            return QuerySingleOrDefault<UserToken>(
                sql: $@"
                    SELECT * FROM {UserTokenSchema.Table}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        [{UserTokenSchema.Columns.Name}] = @{nameof(key.Name)}",
                param: key
            );
        }

        public void DeleteByKey(IUserTokenKey key)
        {
            Execute(
                sql: $@"
                    DELETE FROM {UserTokenSchema.Table}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(key.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(key.LoginProvider)} AND 
                        [{UserTokenSchema.Columns.Name}] = @{nameof(key.Name)}",
                param: key
            );
        }

        public void Update(IUserToken entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE {UserTokenSchema.Table} SET 
                        {UserTokenSchema.Columns.Value} = @{nameof(entity.Value)},
                        {UserTokenSchema.Columns.DateUpdatedUtc} = {nameof(entity.DateUpdatedUtc)}
                    WHERE 
                        {UserTokenSchema.Columns.UserId} = @{nameof(entity.UserId)} AND 
                        {UserTokenSchema.Columns.LoginProvider} = @{nameof(entity.LoginProvider)} AND 
                        [{UserTokenSchema.Columns.Name}] = @{nameof(entity.Name)}",
                param: entity
            );
        }
    }
}