using Store.Common.Extensions;
using Store.Entities.Identity;

namespace Store.DAL.Schema.Identity
{
    public class ClientSchema
    {
        public static string Table { get; } = "identity.client";

        public static class Columns
        {
            public static string Id { get; } = nameof(ClientEntity.Id).ToSnakeCase();
            public static string Name { get; } = nameof(ClientEntity.Name).ToSnakeCase();
            public static string Secret { get; } = nameof(ClientEntity.Secret).ToSnakeCase();
            public static string Description { get; } = nameof(ClientEntity.Description).ToSnakeCase();
            public static string ApplicationType { get; } = nameof(ClientEntity.ApplicationType).ToSnakeCase();
            public static string Active { get; } = nameof(ClientEntity.Active).ToSnakeCase();
            public static string AccessTokenLifeTime { get; } = nameof(ClientEntity.AccessTokenLifeTime).ToSnakeCase();
            public static string RefreshTokenLifeTime { get; } = nameof(ClientEntity.RefreshTokenLifeTime).ToSnakeCase();
            public static string AllowedOrigin { get; } = nameof(ClientEntity.AllowedOrigin).ToSnakeCase();
            public static string DateCreatedUtc { get; } = nameof(ClientEntity.DateCreatedUtc).ToSnakeCase();
            public static string DateUpdatedUtc { get; } = nameof(ClientEntity.DateUpdatedUtc).ToSnakeCase();
        }
    }
}