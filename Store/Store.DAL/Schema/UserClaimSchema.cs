using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public class UserClaimSchema
    {
        public static string Table { get; } = "identity.user_claim";

        public static class Columns
        {
            public static string Id { get; } = nameof(UserClaimEntity.Id).ToSnakeCase();
            public static string UserId { get; } = nameof(UserClaimEntity.UserId).ToSnakeCase();
            public static string ClaimType { get; } = nameof(UserClaimEntity.ClaimType).ToSnakeCase();
            public static string ClaimValue { get; } = nameof(UserClaimEntity.ClaimValue).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserClaimEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(UserClaimEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}