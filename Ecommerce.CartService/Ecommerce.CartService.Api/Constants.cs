namespace Ecommerce.CartService.Api;

public class Constants
{
    public const string V1 = "1.0";
    public const string V2 = "2.0";

    public static class AuthConstants
    {
        public const string RealmAccess = "realm_access";
        public const string Roles = "roles";

        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string StoreCustomerRole = "StoreCustomerRole";
        public const string ManagerAdminOnlyPolicy = "ManagerAdminOnly";
        public const string StoreCustomerManagerOnlyPolicy = "StoreCustomerManagerOnlyPolicy";
    }
}
