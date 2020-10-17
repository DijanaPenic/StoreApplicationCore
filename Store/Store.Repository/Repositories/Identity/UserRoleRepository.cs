using System;
using System.Data;
using System.Threading.Tasks;
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

        public Task AddAsync(Guid userId, string roleName)
        {
            DateTime dateCreated = DateTime.UtcNow;

            return ExecuteAsync(
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

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(Guid userId)
        {
            return await QueryAsync<string>(
                sql: $@"
                    SELECT r.{RoleSchema.Columns.NormalizedName}
                    FROM {UserRoleSchema.Table} ur 
                        INNER JOIN {RoleSchema.Table} r ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id}
                    WHERE ur.{UserRoleSchema.Columns.UserId} = @{nameof(userId)}
                ",
                param: new { userId }
            );
        }

        public async Task<IEnumerable<IUser>> GetUsersByRoleNameAsync(string roleName)
        {
            return await QueryAsync<User>(
                sql: $@"
                    SELECT u.* FROM {UserRoleSchema.Table} ur 
                        INNER JOIN {RoleSchema.Table} r ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id} 
                        INNER JOIN {UserSchema.Table} u ON ur.{UserRoleSchema.Columns.UserId} = u.{UserSchema.Columns.Id}
                    WHERE r.{RoleSchema.Columns.NormalizedName} = @{nameof(roleName)}
                ",
                param: new { roleName });
        }

        public Task DeleteAsync(Guid userId, string roleName)
        {
            return ExecuteAsync(
                sql: $@"
                    DELETE FROM {UserRoleSchema.Table} ur 
                        USING {RoleSchema.Table} r
                    WHERE 
                        ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id} AND
                        r.{RoleSchema.Columns.NormalizedName} = @{nameof(roleName)} AND 
                        ur.{UserRoleSchema.Columns.UserId} = @{nameof(userId)}
                ",
                param: new { userId, roleName }
            );
        }
    }
}