using System;
using System.Data;
using System.Collections.Generic;

using Store.DAL.Schema;
using Store.Common.Helpers;
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
            entity.Id = GuidHelper.NewSequentialGuid();

            Execute(
                sql: $@"
                    INSERT INTO {RoleSchema.Table}(
                        {RoleSchema.Columns.Id}, 
                        {RoleSchema.Columns.ConcurrencyStamp}, 
                        {RoleSchema.Columns.Name}, 
                        {RoleSchema.Columns.NormalizedName},
                        {RoleSchema.Columns.Stackable},
                        {RoleSchema.Columns.DateCreatedUtc},
                        {RoleSchema.Columns.DateUpdatedUtc})
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
                sql: $"SELECT * FROM {RoleSchema.Table}"
            );
        }

        public IRole FindByKey(Guid key)
        {
            return QuerySingleOrDefault<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IRole FindByName(string roleName)
        {
            return QuerySingleOrDefault<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Name} = @{nameof(roleName)}",
                param: new { roleName }
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IRole entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE {RoleSchema.Table} SET 
                        {RoleSchema.Columns.ConcurrencyStamp} = @{nameof(entity.ConcurrencyStamp)}, 
                        {RoleSchema.Columns.Name} = @{nameof(entity.Name)}, 
                        {RoleSchema.Columns.NormalizedName} = @{nameof(entity.NormalizedName)},
                        {RoleSchema.Columns.Stackable} = @{nameof(entity.Stackable)},
                        {RoleSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE {RoleSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}