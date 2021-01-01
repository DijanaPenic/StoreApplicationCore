using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema.Identity
{
    public class UserLoginSchema
    {
        public static string Table { get; } = "identity.user_login";

        public static class Columns
        {
            public static string LoginProvider { get; } = nameof(UserLoginEntity.LoginProvider).ToSnakeCase();
            public static string ProviderKey { get; } = nameof(UserLoginEntity.ProviderKey).ToSnakeCase();
            public static string ProviderDisplayName { get; } = nameof(UserLoginEntity.ProviderDisplayName).ToSnakeCase();
            public static string Token { get; } = nameof(UserLoginEntity.Token).ToSnakeCase();
            public static string IsConfirmed { get; } = nameof(UserLoginEntity.IsConfirmed).ToSnakeCase();
            public static string UserId { get; } = nameof(UserLoginEntity.UserId).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserLoginEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(UserLoginEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}