using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;

namespace PosGo.Contract.Services.V1.RestaurantUser;

public static class Command
{
    public record UpsertRestaurantUserCommand(
        Guid RestaurantId,
        Guid UserId,
        Guid RoleCode // "Owner" | "Manager" | "Staff"
    ) : ICommand;

    public record ChangeRestaurantUserStatusCommand(
        Guid RestaurantId,
        Guid UserId,
        ERestaurantUserStatus Status
    ) : ICommand<Result>;
}
