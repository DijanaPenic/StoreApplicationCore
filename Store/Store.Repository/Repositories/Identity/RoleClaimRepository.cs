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
                    INSERT INTO RoleClaim(
                        {nameof(RoleClaimEntity.ClaimType)}, 
                        {nameof(RoleClaimEntity.ClaimValue)}, 
                        {nameof(RoleClaimEntity.RoleId)},
                        {nameof(RoleClaimEntity.DateCreatedUtc)},
                        {nameof(RoleClaimEntity.DateUpdatedUtc)})
                    VALUES(
                        @{nameof(entity.ClaimType)}, 
                        @{nameof(entity.ClaimValue)}, 
                        @{nameof(entity.RoleId)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)});
                    RETURNING {nameof(RoleClaimEntity.Id)};",
                param: entity
            );
        }

        public IRoleClaim FindByKey(Guid key)
        {
            return QuerySingleOrDefault<RoleClaim>(
                sql: $"SELECT * FROM RoleClaim WHERE {nameof(RoleClaimEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IEnumerable<IRoleClaim> FindByRoleId(string roleId)
        {
            return Query<RoleClaim>(
                sql: $"SELECT * FROM RoleClaim WHERE {nameof(RoleClaimEntity.RoleId)} = @{nameof(roleId)}",
                param: new { roleId }
            );
        }

        public IEnumerable<IRoleClaim> Get()
        {
            return Query<RoleClaim>(
                sql: $"SELECT * FROM RoleClaim"
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM RoleClaim WHERE {nameof(RoleClaimEntity.Id)} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IRoleClaim entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
                sql: $@"
                    UPDATE RoleClaim SET 
                        {nameof(RoleClaimEntity.ClaimType)} = @{nameof(entity.ClaimType)}, 
                        {nameof(RoleClaimEntity.ClaimValue)} = @{nameof(entity.ClaimType)}, 
                        {nameof(RoleClaimEntity.RoleId)} = @{nameof(entity.ClaimType)},
                        {nameof(RoleClaimEntity.DateUpdatedUtc)} = {nameof(entity.DateUpdatedUtc)}
                    WHERE Id = @Id",
                param: entity
            );
        }
    }
}
