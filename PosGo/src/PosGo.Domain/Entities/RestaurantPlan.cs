using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class RestaurantPlan : Entity<int>
{
    public Guid RestaurantId { get; set; }
    public Guid PlanId { get; set; }
    public bool IsActive { get; set; }
    public virtual Restaurant Restaurant { get; set; }
    public virtual Plan Plan { get; set; }
}
