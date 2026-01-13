using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Plan : AuditableEntity<int>
{
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public virtual ICollection<RestaurantPlan> RestaurantPlans { get; private set; }
    public virtual ICollection<PlanFunction> PlanFunctions { get; private set; }

    public Plan(string code, string? description, bool isActive)
    {
        Code = code;
        Description = description;
        IsActive = isActive;
    }

    public static Plan Create(string code, string? description, bool isActive = true)
    {
        return new Plan(code, description, isActive);
    }

    public void Update(string code, string? description, bool isActive)
    {
        Code = code.Trim().ToLowerInvariant();
        Description = description;
        IsActive = isActive;
    }
}
