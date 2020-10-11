using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public static class UserSchema
    {
        public static string Table { get; } = "user";

        public static class Columns
        {
            public static string Id { get; } = nameof(UserEntity.Id).ToSnakeCase();
            public static string UserName { get; } = nameof(UserEntity.UserName).ToSnakeCase();
            public static string NormalizedUserName { get; } = nameof(UserEntity.NormalizedUserName).ToSnakeCase();
            public static string FirstName { get; } = nameof(UserEntity.FirstName).ToSnakeCase();
            public static string LastName { get; } = nameof(UserEntity.LastName).ToSnakeCase();
            public static string Email { get; } = nameof(UserEntity.Email).ToSnakeCase();
            public static string EmailConfirmed { get; } = nameof(UserEntity.EmailConfirmed).ToSnakeCase();
            public static string NormalizedEmail { get; } = nameof(UserEntity.NormalizedEmail).ToSnakeCase();
            public static string PhoneNumber { get; } = nameof(UserEntity.PhoneNumber).ToSnakeCase();
            public static string PhoneNumberConfirmed { get; } = nameof(UserEntity.PhoneNumberConfirmed).ToSnakeCase();
            public static string PasswordHash { get; } = nameof(UserEntity.PasswordHash).ToSnakeCase();
            public static string TwoFactorEnabled { get; } = nameof(UserEntity.TwoFactorEnabled).ToSnakeCase();
            public static string LockoutEnabled { get; } = nameof(UserEntity.LockoutEnabled).ToSnakeCase();
            public static string IsDeleted { get; } = nameof(UserEntity.IsDeleted).ToSnakeCase();
            public static string IsApproved { get; } = nameof(UserEntity.IsApproved).ToSnakeCase();
            public static string AccessFailedCount { get; } = nameof(UserEntity.AccessFailedCount).ToSnakeCase();
            public static string ConcurrencyStamp { get; } = nameof(UserEntity.ConcurrencyStamp).ToSnakeCase();
            public static string SecurityStamp { get; } = nameof(UserEntity.SecurityStamp).ToSnakeCase();
            public static string LockoutEndDateUtc { get; } = nameof(UserEntity.LockoutEndDateUtc).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(UserEntity.Id).ToSnakeCase();
        }   
    }
}