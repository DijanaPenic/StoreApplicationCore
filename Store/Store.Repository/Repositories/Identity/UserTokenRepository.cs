using System;
using System.Data;
using System.Collections.Generic;

using Store.Entities.Identity;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Core.Dapper;

namespace Store.Repository.Repositories.Identity
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
                    INSERT INTO UserToken(
                        {nameof(UserTokenEntity.UserId)}, 
                        {nameof(UserTokenEntity.LoginProvider)}, 
                        [{nameof(UserTokenEntity.Name)}], 
                        {nameof(UserTokenEntity.Value)},
                        {nameof(UserTokenEntity.DateCreatedUtc)},
                        {nameof(UserTokenEntity.DateUpdatedUtc)})
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
                sql: $"SELECT * FROM UserToken"
            );
        }

        public IUserToken FindByKey(IUserTokenKey key)
        {
            return QuerySingleOrDefault<UserToken>(
                sql: $@"
                    SELECT * FROM UserToken
                    WHERE 
                        {nameof(UserTokenEntity.UserId)} = @{nameof(key.UserId)} AND 
                        {nameof(UserTokenEntity.LoginProvider)} = @{nameof(key.LoginProvider)} AND 
                        [{nameof(UserTokenEntity.Name)}] = @{nameof(key.Name)}",
                param: key
            );
        }

        public void DeleteByKey(IUserTokenKey key)
        {
            Execute(
                sql: $@"
                    DELETE FROM UserToken
                    WHERE 
                        {nameof(UserTokenEntity.UserId)} = @{nameof(key.UserId)} AND 
                        {nameof(UserTokenEntity.LoginProvider)} = @{nameof(key.LoginProvider)} AND 
                        [{nameof(UserTokenEntity.Name)}] = @{nameof(key.Name)}",
                param: key
            );
        }

        public void Update(IUserToken entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE UserToken SET 
                        {nameof(UserTokenEntity.Value)} = @{nameof(entity.Value)},
                        {nameof(UserTokenEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}
                    WHERE 
                        {nameof(UserTokenEntity.UserId)} = @{nameof(entity.UserId)} AND 
                        {nameof(UserTokenEntity.LoginProvider)} = @{nameof(entity.LoginProvider)} AND 
                        [{nameof(UserTokenEntity.Name)}] = @{nameof(entity.Name)}",
                param: entity
            );
        }
    }
}