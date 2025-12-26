namespace PosGo.Domain.Utilities.Constants;

public static class PermissionConstants
{
    public const int Deny = -1;

    public const string ManageRestaurantGroups = nameof(ManageRestaurantGroups);
    public const string ManageRestaurants = nameof(ManageRestaurants);
    public const string ManageUsers = nameof(ManageUsers);
    public const string Dashboard = nameof(Dashboard);
    public const string Report = nameof(Report);
    public const string ManageOrders = nameof(ManageOrders);
    public const string ManageTables = nameof(ManageTables);
    public const string ManageDishes = nameof(ManageDishes);
    public const string ManageUnits = nameof(ManageUnits);
    public const string ManageEmployees = nameof(ManageEmployees);
    public const string SwitchBranch = nameof(SwitchBranch);

    public static readonly List<string> PermissionKeys = new List<string>
    {
        ManageRestaurantGroups,
        ManageRestaurants,
        ManageUsers,
        Dashboard,
        Report,
        ManageOrders,
        ManageTables,
        ManageDishes,
        ManageUnits,
        ManageEmployees,
        SwitchBranch,
    };

    public static readonly List<string> Urls = new List<string>
    {
        @"menu/" + ManageRestaurantGroups,
        @"menu/" + ManageRestaurants,
        @"menu/" + ManageUsers,
        @"menu/" + Dashboard,
        @"menu/" + Report,
        @"menu/" + ManageOrders,
        @"menu/" + ManageTables,
        @"menu/" + ManageDishes,
        @"menu/" + ManageUnits,
        @"menu/" + ManageEmployees,
        @"menu/" + SwitchBranch,
    };
}
