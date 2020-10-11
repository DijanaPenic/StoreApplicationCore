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
    internal class UserRoleRepository : DapperRepositoryBase, IUserRoleRepository
    {
        public UserRoleRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public void Add(Guid userId, string roleName)
        {
            DateTime dateCreated = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO {UserRoleSchema.Table}(
                        {UserRoleSchema.Columns.UserId}, 
                        {UserRoleSchema.Columns.RoleId},
                        {UserRoleSchema.Columns.DateCreatedUtc})
                    SELECT
                        @{nameof(userId)}, 
                        {RoleSchema.Columns.Id},
                        @{nameof(dateCreated)}
                    FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.NormalizedName} = @{nameof(roleName)}
                    LIMIT 1",
                param: new { userId, roleName, dateCreated }
            );
        }

        public IEnumerable<string> GetRoleNamesByUserId(Guid userId)
        {
            return Query<string>(
                sql: $@"
                    SELECT r.{RoleSchema.Columns.Name}
                    FROM {UserRoleSchema.Table} ur 
                        INNER JOIN {RoleSchema.Table} r ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id}
                    WHERE ur.{UserRoleSchema.Columns.UserId} = @{nameof(userId)}
                ",
                param: new { userId }
            );
        }

        public IEnumerable<IUser> GetUsersByRoleName(string roleName)
        {
            return Query<User>(
                sql: $@"
                    SELECT u.* FROM {UserRoleSchema.Table} ur 
                        INNER JOIN {RoleSchema.Table} r ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id} 
                        INNER JOIN {UserSchema.Table} u ON ur.{UserRoleSchema.Columns.UserId} = u.{UserSchema.Columns.Id}
                    WHERE r.{RoleSchema.Columns.NormalizedName} = @{nameof(roleName)}
                ",
                param: new { roleName });
        }

        public void Delete(Guid userId, string roleName)
        {
            Execute(
                sql: $@"
                    DELETE FROM {UserRoleSchema.Table} ur 
                        INNER JOIN {RoleSchema.Table} r ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id}
                    WHERE 
                        r.{RoleSchema.Columns.NormalizedName} = @{nameof(roleName)} AND 
                        ur.{UserRoleSchema.Columns.UserId} = @{nameof(userId)}
                ",
                param: new { userId, roleName }
            );
        }
    }
}