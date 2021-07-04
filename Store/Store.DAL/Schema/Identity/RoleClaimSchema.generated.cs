/** Generated by Liquid **/

namespace Store.DAL.Schema.Identity
{
    public static class RoleClaimSchema
    {
        public const string Table = "identity.role_claim";

        public static class Columns
        {    
            public static string Id = "id";
            public static string RoleId = "role_id";
            public static string Role = "role";
            public static string ClaimType = "claim_type";
            public static string ClaimValue = "claim_value";
            public static string DateCreatedUtc = "date_created_utc";
            public static string DateUpdatedUtc = "date_updated_utc";
        }
    }
}