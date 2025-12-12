using System.Security;
using PosGo.Domain.Abstractions.Aggregates;

namespace PosGo.Domain.Entities;

// =====================================
//  CORE / TENANT
// =====================================
public class RestaurantGroup : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public virtual ICollection<Restaurant> Restaurants { get; set; }
}
