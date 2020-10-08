using System;
using System.Data;
using System.Collections.Generic;

using Store.Entities.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repository.Repositories.Identity
{
    internal class RoleRepository : DapperRepositoryBase, IRoleRepository
    {
        public RoleRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public void Add(IRole entity)
        {
            Execute(
                sql: $@"
                    INSERT INTO AspNetRoles({nameof(RoleEntity.Id)}, {nameof(RoleEntity.ConcurrencyStamp)}, [{nameof(RoleEntity.Name)}], {nameof(RoleEntity.NormalizedName)})
                    VALUES(@{nameof(IRole.Id)}, @{nameof(IRole.ConcurrencyStamp)}, @{nameof(IRole.Name)}, @{nameof(IRole.NormalizedName)})",
                param: entity
            );
        }

        public IEnumerable<IRole> Get()
        {
            return Query<IRole>(
                sql: "SELECT * FROM AspNetRoles"
            );
        }

        public IRole FindById(Guid id)
        {
            return QuerySingleOrDefault<IRole>(
                sql: $"SELECT * FROM AspNetRoles WHERE {nameof(RoleEntity.Id)} = @{nameof(id)}",
                param: new { id }
            );
        }

        public IRole FindByName(string roleName)
        {
            return QuerySingleOrDefault<IRole>(
                sql: $"SELECT * FROM AspNetRoles WHERE [{nameof(RoleEntity.Name)}] = @{nameof(roleName)}",
                param: new { roleName }
            );
        }


        public void DeleteById(Guid id)
        {
            Execute(
                sql: $"DELETE FROM AspNetRoles WHERE {nameof(RoleEntity.Id)} = @{nameof(id)}",
                param: new { id }
            );
        }

        public void Update(IRole entity)
        {
            Execute(
                sql: $@"
                    UPDATE AspNetRoles SET 
                        {nameof(RoleEntity.ConcurrencyStamp)} = @{nameof(IRole.ConcurrencyStamp)}, 
                        [{nameof(RoleEntity.Name)}] = @{nameof(IRole.Name)}, 
                        {nameof(RoleEntity.NormalizedName)} = @{nameof(IRole.NormalizedName)}
                    WHERE Id = @Id",
                param: entity
            );
        }
    }
}