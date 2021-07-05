/** Generated by Liquid **/

namespace Store.DAL.Schema.Identity
{
    public static class UserTokenSchema
    {
        public const string Table = "identity.user_token";

        public static class Columns
        {    
            public static string UserId = "user_id";
            public static string User = "user";
            public static string LoginProvider = "login_provider";
            public static string Name = "name";
            public static string Value = "value";
            public static string DateCreatedUtc = "date_created_utc";
            public static string DateUpdatedUtc = "date_updated_utc";
        }
    }
}