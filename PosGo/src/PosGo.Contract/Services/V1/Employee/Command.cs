using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.Employee.Response;

namespace PosGo.Contract.Services.V1.Employee;

public static class Command
{
    public record CreateStaffCommand(
        string UserName,
        string Password,
        string ConfirmPassword,
        string FullName,
        string? PhoneNumber
    ) : ICommand<StaffResponse>;

    public record StaffPermissionItem(
        string PermissionKey,  // VD: "ManageOrders"
        int ActionValue        // VD: 3 (View + Add), không được vượt quá Function.ActionValue
    );

    public record UpdateStaffPermissionsCommand(
        Guid StaffId,
        List<StaffPermissionItem> Permissions
    ) : ICommand;

    public record RemoveStaffPermissionCommand(
        Guid StaffId,
        string PermissionKey
    ) : ICommand;

    public record TransferStaffCommand(
        Guid StaffId,
        Guid FromRestaurantId,
        Guid ToRestaurantId
    ) : ICommand;
}
