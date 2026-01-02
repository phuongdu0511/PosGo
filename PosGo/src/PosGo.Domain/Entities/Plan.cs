using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
using System.Xml.Linq;
using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class Plan : AuditableEntity<Guid>
{
    public string Code { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public virtual ICollection<RestaurantPlan> RestaurantPlans { get; private set; }
    public virtual ICollection<PlanFunction> PlanFunctions { get; private set; }

    public Plan(Guid id, string code, string? description, bool isActive)
    {
        Id = id;
        Code = code;
        Description = description;
        IsActive = isActive;
    }

    public static Plan Create(Guid id, string code, string? description, bool isActive = true)
    {
        return new Plan(id, code, description, isActive);
    }
}
