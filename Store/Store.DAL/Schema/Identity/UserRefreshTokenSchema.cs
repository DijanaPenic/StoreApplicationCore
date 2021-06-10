using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema.Identity
{
    public class UserRefreshTokenSchema
    {
        public static string Table { get; } = "identity.user_refresh_token";

        public static class Columns
        {
            public static string Value { get; } = nameof(UserRefreshTokenEntity.Value).ToSnakeCase();
            public static string UserId { get; } = nameof(UserRefreshTokenEntity.UserId).ToSnakeCase();
            public static string ClientId { get; } = nameof(UserRefreshTokenEntity.ClientId).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserRefreshTokenEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(UserRefreshTokenEntity.DateUpdatedUtc).ToSnakeCase();
            public static string DateExpiresUtc { get; } = nameof(UserRefreshTokenEntity.DateExpiresUtc).ToSnakeCase();
        }
    }
}