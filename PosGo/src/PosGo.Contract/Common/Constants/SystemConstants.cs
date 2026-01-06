namespace PosGo.Contract.Common.Constants;

public static class SystemConstants
{
    public static class Scope
    {
        public const string SYSTEM = "SYSTEM";
        public const string RESTAURANT = "RESTAURANT";
    }

    public static class RoleName
    {
        public const string ADMIN = "SystemAdmin";
        public const string OWNER = "Owner";
        public const string MANAGER = "Manager";
        public const string STAFF = "Staff";
    }

    public static class RoleCode
    {
        public const string ADMIN = "systemadmin";
        public const string OWNER = "owner";
        public const string MANAGER = "manager";
        public const string STAFF = "staff";
    }

    public static class ClaimTypes
    {
        public const string SCOPE = "scope";
        public const string RESTAURANT_ID = "restaurant_id";
    }
}