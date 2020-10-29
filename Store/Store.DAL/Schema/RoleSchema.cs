using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public class RoleSchema
    {
        public static string Table { get; } = "identity.role";

        public static class Columns
        {
            public static string Id { get; } = nameof(RoleEntity.Id).ToSnakeCase();
            public static string Name { get; } = nameof(RoleEntity.Name).ToSnakeCase();
            public static string NormalizedName { get; } = nameof(RoleEntity.NormalizedName).ToSnakeCase();
            public static string ConcurrencyStamp { get; } = nameof(RoleEntity.ConcurrencyStamp).ToSnakeCase();
            public static string Stackable { get; } = nameof(RoleEntity.Stackable).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(RoleEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(RoleEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}