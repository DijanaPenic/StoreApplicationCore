namespace Store.Common.Helpers.Identity
{
    public static class RoleHelper
    {
        public const string Guest = "GUEST";
        public const string Admin = "ADMIN";
        public const string Customer = "CUSTOMER";
        public const string StoreManager = "STORE MANAGER";
        public const string All = "ADMIN,CUSTOMER,STORE MANAGER,GUEST";
    }
}