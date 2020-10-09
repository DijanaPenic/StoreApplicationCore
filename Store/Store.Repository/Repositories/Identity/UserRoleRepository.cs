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
    internal class UserRoleRepository : DapperRepositoryBase, IUserRoleRepository
    {
        public UserRoleRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public void Add(string userId, string roleName)
        {
            DateTime dateCreated = DateTime.UtcNow;

            Execute(
                sql: $@"
                    INSERT INTO UserRole(
                        {nameof(UserRoleEntity.UserId)}, 
                        {nameof(UserRoleEntity.RoleId)},
                        {nameof(UserRoleEntity.DateCreatedUtc)})
                    SELECT TOP 1 
                        @{nameof(userId)}, 
                        {nameof(RoleEntity.Id)},
                        @{nameof(dateCreated)}
                    FROM Role WHERE {nameof(RoleEntity.NormalizedName)} = @{nameof(roleName)}",
                param: new { userId, roleName, dateCreated }
            );
        }

        public IEnumerable<string> GetRoleNamesByUserId(string userId)
        {
            return Query<string>(
                sql: $@"
                    SELECT r.[{nameof(RoleEntity.Name)}]
                    FROM UserRole ur 
                        INNER JOIN Role r ON ur.{nameof(UserRoleEntity.RoleId)} = r.{nameof(RoleEntity.Id)}
                    WHERE ur.{nameof(UserRoleEntity.UserId)} = @{nameof(userId)}
                ",
                param: new { userId }
            );
        }

        public IEnumerable<IUser> GetUsersByRoleName(string roleName)
        {
            return Query<User>(
                sql: $@"
                    SELECT u.* FROM UserRole ur 
                        INNER JOIN Role r ON ur.{nameof(UserRoleEntity.RoleId)} = r.{nameof(RoleEntity.Id)} 
                        INNER JOIN User u ON ur.{nameof(UserRoleEntity.UserId)} = u.{nameof(UserEntity.Id)}
                    WHERE r.{nameof(RoleEntity.NormalizedName)} = @{nameof(roleName)}
                ",
                param: new { roleName });
        }

        public void Delete(string userId, string roleName)
        {
            Execute(
                sql: $@"
                    DELETE FROM UserRole ur 
                        INNER JOIN Role r ON ur.{nameof(UserRoleEntity.RoleId)} = r.{nameof(RoleEntity.Id)}
                    WHERE 
                        r.{nameof(RoleEntity.NormalizedName)} = @{nameof(roleName)} AND 
                        ur.{nameof(UserRoleEntity.UserId)} = @{nameof(userId)}
                ",
                param: new { userId, roleName }
            );
        }
    }
}