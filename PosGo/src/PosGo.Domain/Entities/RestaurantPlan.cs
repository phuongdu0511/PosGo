using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class RestaurantPlan : AuditableEntity<int>
{
    public Guid RestaurantId { get; private set; }
    public Guid PlanId { get; private set; }
    public bool IsActive { get; private set; }
    public virtual Restaurant Restaurant { get; private set; }
    public virtual Plan Plan { get; private set; }

    public RestaurantPlan(Guid restaurantId, Guid planId)
    {
        RestaurantId = restaurantId;
        PlanId = planId;
        IsActive = true;
    }

    // Factory method: chỗ khác muốn tạo phải đi qua đây
    public static RestaurantPlan Create(Guid restaurantId, Guid planId)
        => new RestaurantPlan(restaurantId, planId);

    // Đổi trạng thái
    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
