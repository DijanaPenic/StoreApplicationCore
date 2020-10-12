using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Store.DAL.Schema;
using Store.Common.Helpers;
using Store.Model.Models.Identity;
using Store.Model.Common.Models.Identity;
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

        public void Add(IUser entity)
        {
            entity.DateCreatedUtc = DateTime.UtcNow;
            entity.DateUpdatedUtc = DateTime.UtcNow;
            entity.Id = GuidHelper.NewSequentialGuid();

            Execute(
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

        public IEnumerable<IUser> Get()
        {
            return Query<User>(
                sql: $"SELECT * FROM {UserSchema.Table}"
            );
        }

        public IUser FindByKey(Guid key)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public IUser FindByKey(Guid key, params string[] includeProperties)
        {
            StringBuilder sql = new StringBuilder($"SELECT * FROM {UserSchema.Table} u WHERE {UserSchema.Columns.Id} = @{nameof(key)};");

            if(includeProperties.Contains(nameof(IUser.Roles)))
            {
                sql.Append($@"SELECT r.* FROM {RoleSchema.Table} r
                          INNER JOIN {UserRoleSchema.Table} ur ON ur.{UserRoleSchema.Columns.RoleId} = r.{RoleSchema.Columns.Id}
                          WHERE ur.{UserRoleSchema.Columns.UserId} = @{nameof(key)};");
            }
            // TODO - add other navigation properties

            using GridReader reader = QueryMultiple(sql.ToString(), param: new { key });

            IUser user = reader.Read<User>().FirstOrDefault();

            // Populate navigation properties
            if (user != null && includeProperties?.Count() > 0)
            {
                if (includeProperties.Contains(nameof(IUser.Roles))) user.Roles = reader.Read<Role>().ToList<IRole>();
            }

            return user;
        }

        public IUser FindByNormalizedEmail(string normalizedEmail)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.NormalizedEmail} = @{nameof(normalizedEmail)}",
                param: new { normalizedEmail }
            );
        }

        public IUser FindByNormalizedUserName(string normalizedUserName)
        {
            return QuerySingleOrDefault<User>(
                sql: $"SELECT * FROM {UserSchema.Table} WHERE {UserSchema.Columns.NormalizedUserName} = @{nameof(normalizedUserName)}",
                param: new { normalizedUserName }
            );
        }

        public void DeleteByKey(Guid key)
        {
            Execute(
                sql: $"DELETE FROM {UserSchema.Table} WHERE {UserSchema.Columns.Id} = @{nameof(key)}",
                param: new { key }
            );
        }

        public void Update(IUser entity)
        {
            entity.DateUpdatedUtc = DateTime.UtcNow;

            Execute(
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
    }
}