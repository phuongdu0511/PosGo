using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  CORE / TENANT
// =====================================
public class RestaurantLanguage : Entity<Guid>
{
    public Guid RestaurantId { get; private set; }
    public Guid LanguageId { get; private set; }
    public bool IsEnabled { get; private set; }

    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Language Language { get; private set; } = null!;
}
