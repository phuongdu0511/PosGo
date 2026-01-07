using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  OPENING HOURS
// =====================================
public class RestaurantOpeningHour : AuditableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }    // 0-6
    public TimeSpan? OpenTime { get; private set; }
    public TimeSpan? CloseTime { get; private set; }
    public bool IsClosed { get; private set; }

    public virtual Restaurant Restaurant { get; private set; } = null!;
}
