namespace PosGo.Domain.Abstractions.Entities;

/// <summary>
/// Entity thuộc về 1 Restaurant (Tenant)
/// </summary>
public interface ITenantEntity
{
    Guid RestaurantId { get; }
}
