﻿using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Models;
using Store.Models.Identity;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Helpers;
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
            IList<string> filterConditions = new List<string>();

            if (!showInactive) filterConditions.Add($"u.{UserSchema.Columns.IsApproved} = TRUE");
            if (searchString != null) filterConditions.Add($"((LOWER(u.{UserSchema.Columns.FirstName}) LIKE @{nameof(searchString)}) OR (LOWER(u.{UserSchema.Columns.LastName}) LIKE @{nameof(searchString)}))");

            dynamic searchParameters = new ExpandoObject();
            searchParameters.SearchString = $"%{searchString?.ToLowerInvariant()}%";

            using GridReader reader = await FindAsync
            (
                tableName: UserSchema.Table,
                tableAlias: "u",
                selectAlias: "u.*, r.*, uc.*, ul.*, ut.*", 
                filterExpression: new StringBuilder("WHERE ").AppendJoin(" AND ", filterConditions).ToString(), 
                includeExpression: IncludeQuery(includeProperties), 
                sortOrderProperty: sortOrderProperty,
                isDescendingSortOrder: isDescendingSortOrder,
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchParameters: searchParameters
            );

            IEnumerable<IUser> users = ReadUsers(reader);
            int totalCount = reader.ReadFirst<int>();

            return new PagedEnumerable<IUser>(users, totalCount, pageSize, pageNumber);
        }

        public async Task<IUser> FindByKeyAsync(Guid key, params string[] includeProperties)
        {
            // Set query base
            StringBuilder sql = new StringBuilder(@$"SELECT u.*, r.*, uc.*, ul.*, ut.* FROM {UserSchema.Table} u");
            sql.Append(Environment.NewLine);

            // Set prefetch
            sql.Append(IncludeQuery(includeProperties));
            sql.Append(Environment.NewLine);

            // Set filter
            sql.Append($@"WHERE u.{UserSchema.Columns.Id} = @{nameof(key)};");

            // Execute query and read user
            using GridReader reader = await QueryMultipleAsync(sql.ToString(), param: new { key });
            IUser user = ReadUsers(reader).SingleOrDefault();

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
	                    {UserSchema.Columns.UserName} = @{nameof(entity.UserName)},
                        {UserSchema.Columns.DateUpdatedUtc}  = @{nameof(entity.DateUpdatedUtc)}

                    WHERE {UserSchema.Columns.Id} = @{nameof(entity.Id)}",
                param: entity);
        }

        private static string IncludeQuery(params string[] includeProperties)
        {
            StringBuilder sql = new StringBuilder();

            bool includeRoles = includeProperties.Contains(nameof(IUser.Roles));
            bool includeClaims = includeProperties.Contains(nameof(IUser.Claims));
            bool includeLogins = includeProperties.Contains(nameof(IUser.Logins));
            bool includetokens = includeProperties.Contains(nameof(IUser.Tokens));

            if (includeRoles)
            {
                sql.Append($@"INNER JOIN {UserRoleSchema.Table} ur on ur.{UserRoleSchema.Columns.UserId} = u.{UserSchema.Columns.Id} 
                              INNER JOIN {RoleSchema.Table} r on r.{RoleSchema.Columns.Id} = ur.{UserRoleSchema.Columns.RoleId}");
            }
            else
            {
                sql.Append($@"LEFT JOIN {RoleSchema.Table} r on FALSE");
            }
            sql.Append(Environment.NewLine);

            sql.Append($@"LEFT JOIN {UserClaimSchema.Table} uc on {(includeClaims ? $"uc.{UserClaimSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}
                          LEFT JOIN {UserLoginSchema.Table} ul on {(includeLogins ? $"ul.{UserLoginSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}
                          LEFT JOIN {UserTokenSchema.Table} ut on {(includetokens ? $"ut.{UserTokenSchema.Columns.UserId} = u.{UserSchema.Columns.Id}" : "FALSE")}");

            return sql.ToString();
        }

        private static IEnumerable<IUser> ReadUsers(GridReader reader)
        {
            IEnumerable<IUser> users = reader.Read<User, Role, UserClaim, UserLogin, UserToken, User>((user, userRole, userClaim, userLogin, userToken) =>
            {
                if (userRole != null) user.Roles = new List<IRole>() { userRole };
                if (userClaim != null) user.Claims = new List<IUserClaim>() { userClaim };
                if (userLogin != null) user.Logins = new List<IUserLogin>() { userLogin };
                if (userToken != null) user.Tokens = new List<IUserToken>() { userToken };

                return user;
            }, splitOn: $"{RoleSchema.Columns.Id}, {UserClaimSchema.Columns.Id}, {UserLoginSchema.Columns.LoginProvider}, {UserTokenSchema.Columns.UserId}");

            IEnumerable<IUser> mergedUsers = users.GroupBy(u => u.Id).Select(gu =>
            {
                IUser user = gu.First();

                user.Roles = gu.Where(u => u.Roles != null).Select(u => u.Roles.Single()).ToList();
                user.Claims = gu.Where(u => u.Claims != null).Select(u => u.Claims.Single()).ToList();
                user.Logins = gu.Where(u => u.Logins != null).Select(u => u.Logins.Single()).ToList();
                user.Tokens = gu.Where(u => u.Tokens != null).Select(u => u.Tokens.Single()).ToList();

                return user;
            });

            return mergedUsers;
        }
    }
}