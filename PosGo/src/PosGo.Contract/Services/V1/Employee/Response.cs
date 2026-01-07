using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.Employee;

public static class Response
{
    public record StaffResponse(
        Guid Id,
        string UserName,
        string FullName,
        string? PhoneNumber
    );

    public record StaffDetailResponse(
        Guid Id,
        string UserName,
        string FullName,
        string? PhoneNumber,
        EUserStatus Status,
        Guid RestaurantId,
        string RestaurantName
    );

    public record StaffPermissionResponse(
        string PermissionKey,
        string PermissionName,
        int ActionValue,
        int MaxActionValue,  // Function.ActionValue (giới hạn tối đa)
        List<string> AllowedActions  // ["View", "Add", "Update", "Delete"]
    );
}
