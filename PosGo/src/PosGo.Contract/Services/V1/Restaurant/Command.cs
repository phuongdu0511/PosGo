using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.Restaurant;

public static class Command
{
    public record CreateRestaurantCommand(
        string Name,
        string Slug,
        Guid DefaultLanguageId,
        string? Address,
        string? City,
        string? Country,
        string? Phone,
        string? TimeZone,
        string? LogoUrl,
        string? Description,
        Guid? RestaurantGroupId
    ) : ICommand;

    public record UpdateRestaurantCommand(
        Guid Id,
        string Name,
        string Slug,
        Guid DefaultLanguageId,
        string? Address,
        string? City,
        string? Country,
        string? Phone,
        string? TimeZone,
        string? LogoUrl,
        string? Description,
        Guid? RestaurantGroupId,
        bool IsActive
    ) : ICommand;

    public record UpdateRestaurantPlanCommand(
        Guid RestaurantId, 
        Guid PlanId
    ) : ICommand;

    public sealed record AssignUserToRestaurantCommand(
        Guid RestaurantId,
        Guid UserId,
        Guid RoleId
    ) : ICommand;
}
