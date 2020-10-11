using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public class RoleClaimSchema
    {
        public static string Table { get; } = "role_claim";

        public static class Columns
        {
            public static string Id { get; } = nameof(RoleClaimEntity.Id).ToSnakeCase();
            public static string RoleId { get; } = nameof(RoleClaimEntity.RoleId).ToSnakeCase();
            public static string ClaimType { get; } = nameof(RoleClaimEntity.ClaimType).ToSnakeCase();
            public static string ClaimValue { get; } = nameof(RoleClaimEntity.ClaimValue).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(RoleClaimEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(RoleClaimEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}