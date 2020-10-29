using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema
{
    public class UserRoleSchema
    {
        public static string Table { get; } = "identity.user_role";

        public static class Columns
        {
            public static string UserId { get; } = nameof(UserRoleEntity.UserId).ToSnakeCase();
            public static string RoleId { get; } = nameof(UserRoleEntity.RoleId).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(UserRoleEntity.DateCreatedUtc).ToSnakeCase();
        }
    }
}