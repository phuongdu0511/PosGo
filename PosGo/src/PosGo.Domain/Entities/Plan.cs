using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Plan : AuditableEntity<Guid>
{
    public string Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<RestaurantPlan> RestaurantPlans { get; set; }
    public virtual ICollection<PlanFunction> PlanFunctions { get; set; }
}
