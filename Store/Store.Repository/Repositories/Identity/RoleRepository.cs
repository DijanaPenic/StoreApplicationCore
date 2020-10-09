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
    internal class RoleRepository : DapperRepositoryBase, IRoleRepository
    {
        public RoleRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IRole entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO Role(
                        {nameof(RoleEntity.Id)}, 
                        {nameof(RoleEntity.ConcurrencyStamp)}, 
                        [{nameof(RoleEntity.Name)}], 
                        {nameof(RoleEntity.NormalizedName)},
                        {nameof(RoleEntity.Stackable)},
                        {nameof(RoleEntity.DateCreatedUtc)},
                        {nameof(RoleEntity.DateUpdatedUtc)})
                    VALUES(
                        @{nameof(entity.Id)}, 
                        @{nameof(entity.ConcurrencyStamp)}, 
                        @{nameof(entity.Name)}, 
                        @{nameof(entity.NormalizedName)},
                        @{nameof(entity.Stackable)}, 
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public IEnumerable<IRole> Get()
        {
            return Query<Role>(
                sql: "SELECT * FROM Role"
            );
        }

        public IRole FindByKey(Guid key)
        {
            return QuerySingleOrDefault<Role>(
                sql: $"SELECT * FROM Role WHERE {nameof(RoleEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IRole FindByName(string roleName)
        {
            return QuerySingleOrDefault<Role>(
                sql: $"SELECT * FROM Role WHERE [{nameof(RoleEntity.Name)}] = @{nameof(roleName)}",
                param: new { roleName }
            );
        }


        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM Role WHERE {nameof(RoleEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IRole entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE Role SET 
                        {nameof(RoleEntity.ConcurrencyStamp)} = @{nameof(entity.ConcurrencyStamp)}, 
                        [{nameof(RoleEntity.Name)}] = @{nameof(entity.Name)}, 
                        {nameof(RoleEntity.NormalizedName)} = @{nameof(entity.NormalizedName)},
                        {nameof(RoleEntity.Stackable)} = @{nameof(entity.Stackable)},
                        {nameof(RoleEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}
                    WHERE Id = @Id",
                param: entity
            );
        }
    }
}