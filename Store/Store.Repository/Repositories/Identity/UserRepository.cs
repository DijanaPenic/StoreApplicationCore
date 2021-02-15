using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Common.Helpers;
using Store.Model.Models;
using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

using static Dapper.SqlMapper;

namespace Store.Repositories.Identity
{
    internal class UserRepository : DapperRepositoryBase, IUserRepository
    {
        public UserRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        class Prefetch
        {
            public bool Roles { get; set; }
            public bool Claims { get; set; }
            public bool Logins { get; set; }
            public bool Tokens { get; set; }
        }

        public Task AddAsync(IUser entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteAsync(
                sql: $@"
                    INSERT INTO {UserSchema.Table}(
                        {UserSchema.Columns.Id}, 
                        {UserSchema.Columns.FirstName}, 
                        {UserSchema.Columns.LastName}, 
                        {UserSchema.Columns.AccessFailedCount}, 
                        {UserSchema.Columns.ConcurrencyStamp}, 
                        {UserSchema.Columns.Email}, 
                        {UserSchema.Columns.EmailConfirmed},
	                    {UserSchema.Columns.LockoutEnabled}, 
                        {UserSchema.Columns.LockoutEndDateUtc}, 
                        {UserSchema.Columns.NormalizedEmail}, 
                        {UserSchema.Columns.NormalizedUserName},
	                    {UserSchema.Columns.PasswordHash}, 
                        {UserSchema.Columns.PhoneNumber}, 
                        {UserSchema.Columns.PhoneNumberConfirmed}, 
                        {UserSchema.Columns.SecurityStamp},
	                    {UserSchema.Columns.TwoFactorEnabled}, 
	                    {UserSchema.Columns.IsApproved}, 
	                    {UserSchema.Columns.IsDeleted}, 
                        {UserSchema.Columns.UserName},
                        {UserSchema.Columns.DateCreatedUtc},
                        {UserSchema.Columns.DateUpdatedUtc})
                    VALUES(
                        @{nameof(entity.Id)}, 
                        @{nameof(entity.FirstName)}, 
                        @{nameof(entity.LastName)}, 
                        @{nameof(entity.AccessFailedCount)}, 
                        @{nameof(entity.ConcurrencyStamp)}, 
                        @{nameof(entity.Email)}, 
                        @{nameof(entity.EmailConfirmed)},
	                    @{nameof(entity.LockoutEnabled)}, 
                        @{nameof(entity.LockoutEndDateUtc)}, 
                        @{nameof(entity.NormalizedEmail)}, 
                        @{nameof(entity.NormalizedUserName)},
	                    @{nameof(entity.PasswordHash)}, 
                        @{nameof(entity.PhoneNumber)}, 
                        @{nameof(entity.PhoneNumberConfirmed)}, 
                        @{nameof(entity.SecurityStamp)},
	                    @{nameof(entity.TwoFactorEnabled)}, 
	                    @{nameof(entity.IsApproved)},
	                    @{nameof(entity.IsDeleted)},
                        @{nameof(entity.UserName)},
                        @{nameof(entity.DateCreatedUtc)},
                        @{nameof(entity.DateUpdatedUtc)})",
                param: entity
            );
        }

        public async Task<IEnumerable<IUser>> GetAsync()
        {
            return await QueryAsync<User>(
                sql: $"SELECT * FROM {UserSchema.Table}"
            );
        }

        public async Task<IUser> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IPagedEnumerable<IUser>> FindAsync(string searchString, bool showInactive, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            string searchFilter = @$"u.{UserSchema.Columns.IsDeleted} = FALSE
                                    {(showInactive 
                                        ? string.Empty 
                                        : $"AND u.{UserSchema.Columns.IsApproved} = TRUE")}
                                    {((searchString == null) 
                                        ? string.Empty 
                                        : $"AND ((LOWER(u.{UserSchema.Columns.FirstName}) LIKE @{nameof(searchString)}) OR (LOWER(u.{UserSchema.Columns.LastName}) LIKE @{nameof(searchString)}))")}";

            using GridReader reader = await FindAsync
            (
                table: $"{UserSchema.Table} u", 
                select: "u.*, r.*, uc.*, ul.*, ut.*", 
                searchFilter, 
                include: IncludeQuery(out Prefetch prefetch, includeProperties), 
                searchString, 
                sortOrderProperty: $"u.{sortOrderProperty}", 
                isDescendingSortOrder, 
                pageNumber, 
                pageSize
            );

            IEnumerable<IUser> users = ReadUsers(reader, prefetch);
            int totalCount = reader.ReadFirst<int>();

            return new PagedEnumerable<IUser>(users, totalCount, pageSize, pageNumber);
        }

        public async Task<IUser> FindByKeyAsync(Guid key, params string[] includeProperties)
        {
            // Set query base
            StringBuilder sql = new StringBuilder(@$"SELECT u.*, r.*, uc.*, ul.*, ut.* FROM {UserSchema.Table} u");
            sql.Append(Environment.NewLine);

            // Set prefetch
            sql.Append(IncludeQuery(out Prefetch include, includeProperties));

            // Set filter
            sql.Append($@"WHERE u.{UserSchema.Columns.Id} = @{nameof(key)};");

            // Execute query and read user
            using GridReader reader = await QueryMultipleAsync(sql.ToString(), param: new { key });
            IUser user = ReadUsers(reader, include).FirstOrDefault();

            return user;
        }

        public async Task<IUser> FindByNormalizedEmailAsync(string normalizedEmail)
        {
            return await QuerySingleOrDefaultAsync<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.NormalizedEmail} = @{nameof(normalizedEmail)}",
                param: new { normalizedEmail }
            );
        }

        public async Task<IUser> FindByNormalizedUserNameAsync(string normalizedUserName)
        {
            return await QuerySingleOrDefaultAsync<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.NormalizedUserName} = @{nameof(normalizedUserName)}",
                param: new { normalizedUserName }
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteAsync(
                sql: $"DELETE FROM {UserSchema.Table} WHERE {UserSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task UpdateAsync(IUser entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
                sql: $@"
                    UPDATE {UserSchema.Table} SET 
                        {UserSchema.Columns.FirstName} = @{nameof(entity.FirstName)},
                        {UserSchema.Columns.LastName} = @{nameof(entity.LastName)},
                        {UserSchema.Columns.AccessFailedCount} = @{nameof(entity.AccessFailedCount)},
	                    {UserSchema.Columns.ConcurrencyStamp} = @{nameof(entity.ConcurrencyStamp)}, 
                        {UserSchema.Columns.Email} = @{nameof(entity.Email)},
	                    {UserSchema.Columns.EmailConfirmed} = @{nameof(entity.EmailConfirmed)}, 
                        {UserSchema.Columns.LockoutEnabled} = @{nameof(entity.LockoutEnabled)},
	                    {UserSchema.Columns.LockoutEndDateUtc} = @{nameof(entity.LockoutEndDateUtc)}, 
                        {UserSchema.Columns.NormalizedEmail} = @{nameof(entity.NormalizedEmail)},
	                    {UserSchema.Columns.NormalizedUserName} = @{nameof(entity.NormalizedUserName)}, 
                        {UserSchema.Columns.PasswordHash} = @{nameof(entity.PasswordHash)},
	                    {UserSchema.Columns.PhoneNumber} = @{nameof(entity.PhoneNumber)}, 
                        {UserSchema.Columns.PhoneNumberConfirmed} = @{nameof(entity.PhoneNumberConfirmed)},
	                    {UserSchema.Columns.SecurityStamp} = @{nameof(entity.SecurityStamp)}, 
                        {UserSchema.Columns.TwoFactorEnabled} = @{nameof(entity.TwoFactorEnabled)},
                        {UserSchema.Columns.IsApproved} = @{nameof(entity.IsApproved)},
                        {UserSchema.Columns.IsDeleted} = @{nameof(entity.IsDeleted)},
	                    {UserSchema.Columns.UserName} = @{nameof(entity.UserName)},
                        {UserSchema.Columns.DateUpdatedUtc}  = @{nameof(entity.DateUpdatedUtc)}

                    WHERE {UserSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity);
        }

        private static string IncludeQuery(out Prefetch include, params string[] includeProperties)
        {
            StringBuilder sql = new StringBuilder();

            include = new Prefetch
            {
                Roles = (includeProperties.Contains(nameof(IUser.Roles))),
                Claims = (includeProperties.Contains(nameof(IUser.Claims))),
                Logins = (includeProperties.Contains(nameof(IUser.Logins))),
                Tokens = (includeProperties.Contains(nameof(IUser.Tokens)))
            };

            if (include.Roles)
            {
                sql.Append($@"INNER JOIN {UserRoleSchema.Table} ur on ur.{UserRoleSchema.Columns.UserId} = u.{UserSchema.Columns.Id} 
                              INNER JOIN {RoleSchema.Table} r on r.{RoleSchema.Columns.Id} = ur.{UserRoleSchema.Columns.RoleId}");
            }
            else
            {
                sql.Append($@"LEFT JOIN {RoleSchema.Table} r on FALSE");
            }
            sql.Append(Environment.NewLine);

            sql.Append($@"LEFT JOIN {UserClaimSchema.Table} uc on {(include.Claims ? $"uc.{UserClaimSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}
                          LEFT JOIN {UserLoginSchema.Table} ul on {(include.Logins ? $"ul.{UserLoginSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}
                          LEFT JOIN {UserTokenSchema.Table} ut on {(include.Tokens ? $"ut.{UserTokenSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}");
            sql.Append(Environment.NewLine);

            return sql.ToString();
        }

        private static IEnumerable<IUser> ReadUsers(GridReader reader, Prefetch include)
        {
            IEnumerable<IUser> users = reader.Read<User, Role, UserClaim, UserLogin, UserToken, User>((user, role, userClaim, userLogin, userToken) =>
            {
                if (role != null) user.Roles = new List<IRole>() { role };
                if (userClaim != null) user.Claims = new List<IUserClaim>() { userClaim };
                if (userLogin != null) user.Logins = new List<IUserLogin>() { userLogin };
                if (userToken != null) user.Tokens = new List<IUserToken>() { userToken };

                return user;
            }, splitOn: $"{RoleSchema.Columns.Id}, {UserClaimSchema.Columns.Id}, {UserLoginSchema.Columns.LoginProvider}, {UserTokenSchema.Columns.UserId}");

            IEnumerable<IUser> mergedUsers = users.GroupBy(p => p.Id).Select(gu =>
            {
                IUser user = gu.First();

                if (include.Roles) user.Roles = gu.Select(u => u.Roles.Single()).ToList();
                if (include.Claims) user.Claims = gu.Select(u => u.Claims.Single()).ToList();
                if (include.Logins) user.Logins = gu.Select(u => u.Logins.Single()).ToList();
                if (include.Tokens) user.Tokens = gu.Select(u => u.Tokens.Single()).ToList();

                return user;
            });

            return mergedUsers;
        }
    }
}