using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public class UserTokenSchema
    {
        public static string Table { get; } = "user_token";

        public static class Columns
        {
            public static string UserId { get; } = nameof(UserTokenEntity.UserId).ToSnakeCase();
            public static string LoginProvider { get; } = nameof(UserTokenEntity.LoginProvider).ToSnakeCase();
            public static string Name { get; } = nameof(UserTokenEntity.Name).ToSnakeCase();
            public static string Value { get; } = nameof(UserTokenEntity.Value).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserTokenEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(UserTokenEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}