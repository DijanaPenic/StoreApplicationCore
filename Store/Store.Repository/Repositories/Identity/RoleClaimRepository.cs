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
    internal class RoleClaimRepository : DapperRepositoryBase, IRoleClaimRepository
    {
        public RoleClaimRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IRoleClaim entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;

            entity.Id = ExecuteScalar<Guid>(
                sql: $@"
                    INSERT INTO {RoleClaimSchema.Table}(
                        {RoleClaimSchema.Columns.ClaimType}, 
                        {RoleClaimSchema.Columns.ClaimValue}, 
                        {RoleClaimSchema.Columns.RoleId},
                        {RoleClaimSchema.Columns.DateCreatedUtc},
                        {RoleClaimSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.RoleId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});
                    RETURNING {RoleClaimSchema.Columns.Id};",
                param: entity
            );
        }

        public IRoleClaim FindByKey(Guid key)
        {
            return QuerySingleOrDefault<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IEnumerable<IRoleClaim> FindByRoleId(Guid roleId)
        {
            return Query<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.RoleId} = @{nameof(roleId)}",
                param: new { roleId }
            );
        }

        public IEnumerable<IRoleClaim> Get()
        {
            return Query<RoleClaim>(
                sql: $"SELECT * FROM {RoleClaimSchema.Table}"
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM {RoleClaimSchema.Table} WHERE {RoleClaimSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IRoleClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE {RoleClaimSchema.Table} SET 
                        {RoleClaimSchema.Columns.ClaimType} = @{nameof(entity.ClaimType)}, 
                        {RoleClaimSchema.Columns.ClaimValue} = @{nameof(entity.ClaimType)}, 
                        {RoleClaimSchema.Columns.RoleId} = @{nameof(entity.ClaimType)},
                        {RoleClaimSchema.Columns.DateUpdatedUtc} = @{nameof(entity.DateUpdatedUtc)}
                    WHERE {RoleClaimSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity
            );
        }
    }
}
