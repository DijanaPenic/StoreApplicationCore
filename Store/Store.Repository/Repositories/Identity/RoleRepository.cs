using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using Store.Model.Models;
using Store.Model.Common.Models;
using Store.Model.Common.Models.Identity;
using Store.Common.Helpers;
using Store.Models.Identity;
using Store.DAL.Schema.Identity;
using Store.Repository.Core.Dapper;
using Store.Repository.Common.Repositories.Identity;

using static Dapper.SqlMapper;

namespace Store.Repositories.Identity
{
    internal class RoleRepository : DapperRepositoryBase, IRoleRepository
    {
        public const string CLAIM_PERMISSION_KEY = "Permission";

        public RoleRepository(IDbTransaction transaction) : base(transaction)
        { 
        }

        public Task AddAsync(IRole entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            return ExecuteAsync(
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

        public async Task<IEnumerable<IRole>> GetAsync()
        {
            return await QueryAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table}"
            );
        }

        public async Task<IRole> FindByKeyAsync(Guid key, params string[] includeProperties)
        {
            // Set query base
            StringBuilder sql = new StringBuilder(@$"SELECT r.*, rc.* FROM {RoleSchema.Table} r");
            sql.Append(Environment.NewLine);

            // Set prefetch
            sql.Append(IncludeQuery(includeProperties));
            sql.Append(Environment.NewLine);

            // Set filter
            sql.Append($@"WHERE r.{RoleSchema.Columns.Id} = @{nameof(key)};");

            // Execute query and read user
            using GridReader reader = await QueryMultipleAsync(sql.ToString(), param: new { key });
            IRole role = ReadRoles(reader).SingleOrDefault();

            return role;
        }

        public async Task<IPagedEnumerable<IRole>> FindAsync(string searchString, string sortOrderProperty, bool isDescendingSortOrder, int pageNumber, int pageSize, params string[] includeProperties)
        {
            dynamic searchParameters = new ExpandoObject();
            searchParameters.SearchString = $"%{searchString?.ToLowerInvariant()}%";

            using GridReader reader = await FindAsync
            (
                tableName: RoleSchema.Table,
                tableAlias: "r",
                selectAlias: "r.*, rc.*", 
                filterExpression: $"WHERE (LOWER(r.{RoleSchema.Columns.Name}) LIKE @{nameof(searchString)})",
                includeExpression: IncludeQuery(includeProperties), 
                sortOrderProperty: sortOrderProperty,
                isDescendingSortOrder: isDescendingSortOrder,
                pageNumber: pageNumber,
                pageSize: pageSize,
                searchParameters: searchParameters
            );

            IEnumerable<IRole> roles = ReadRoles(reader);
            int totalCount = reader.ReadFirst<int>();

            var result = new PagedEnumerable<IRole>(roles, totalCount, pageSize, pageNumber);

            return result;
        }

        public async Task<IRole> FindByKeyAsync(Guid key)
        {
            return await QuerySingleOrDefaultAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public async Task<IRole> FindByNameAsync(string roleName)
        {
            return await QuerySingleOrDefaultAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.NormalizedName} = @{nameof(roleName)}",
                param: new { roleName }
            );
        }

        public async Task<IEnumerable<IRole>> FindByNameAsync(string[] roleNames)
        {
            return await QueryAsync<Role>(
                sql: $"SELECT * FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.NormalizedName} = ANY(@{nameof(roleNames)})",
                param: new { roleNames }
            );
        }

        public Task DeleteByKeyAsync(Guid key)
        {
            return ExecuteAsync(
                sql: $"DELETE FROM {RoleSchema.Table} WHERE {RoleSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public Task UpdateAsync(IRole entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            return ExecuteAsync(
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

        private static string IncludeQuery(params string[] includeProperties)
        {
            bool includePolicies = includeProperties.Contains(nameof(IRole.Policies));

            return $@"LEFT JOIN {RoleClaimSchema.Table} rc on {(includePolicies ? 
                            @$"rc.{RoleClaimSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id} 
                            {(includePolicies ? $"AND rc.{RoleClaimSchema.Columns.ClaimType} = '{CLAIM_PERMISSION_KEY}'" : string.Empty)}"
                      : "FALSE")}";
        }

        private static IEnumerable<IRole> ReadRoles(GridReader reader)
        {
            IEnumerable<IRole> roles = reader.Read<Role, RoleClaim, Role>((role, roleClaim) =>
            {
                if (roleClaim?.ClaimType == CLAIM_PERMISSION_KEY) role.Policies = new List<IRoleClaim>() { roleClaim };

                return role;
            }, splitOn: $"{RoleClaimSchema.Columns.Id}");

            IEnumerable<IRole> mergedRoles = roles.GroupBy(r => r.Id).Select(gr =>
            {
                IRole role = gr.First();
                role.Policies = gr.Where(r => r.Policies != null).Select(r => r.Policies.Single()).ToList();

                return role;
            });

            return mergedRoles;
        }

    }
}