using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Store.DAL.Context;
using Store.DAL.Schema.Identity;
using Store.Repository.Core;
using Store.Repository.Common.Repositories.Identity;
using Store.Entities.Identity;
using Store.Model.Common.Models.Identity;

namespace Store.Repositories.Identity
{
    internal class UserRoleRepository : GenericRepository, IUserRoleRepository
    {
        // NOTE - UserRole is configured as shared entity type so we must provide an entity type name.
        private DbSet<UserRoleEntity> DbSet => DbContext.Set<UserRoleEntity>("user_role"); 

        public UserRoleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public Task AddAsync(Guid userId, string roleName)
        {
            DateTime dateCreated = DateTime.UtcNow;

            return ExecuteQueryAsync(
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

        public async Task<IEnumerable<string>> FindRolesByUserIdAsync(Guid userId)
        {
            return await DbSet.Where(ur => ur.UserId == userId).Select(ur => ur.Role.NormalizedName).ToListAsync();
        }

        public async Task<IEnumerable<IUser>> FindUsersByRoleNameAsync(string roleName)
        {
            List<UserEntity> entities = await DbSet.Where(ur => ur.Role.NormalizedName == roleName).Select(ur => ur.User).ToListAsync();

            return Mapper.Map<IEnumerable<IUser>>(entities);
        }

        public Task<int> GetCountByRoleNameAsync(string roleName)
        {
            return DbSet.Where(ur => ur.Role.NormalizedName == roleName).CountAsync();
        }

        public Task DeleteAsync(Guid userId, string roleName)
        {
            return ExecuteQueryAsync(
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