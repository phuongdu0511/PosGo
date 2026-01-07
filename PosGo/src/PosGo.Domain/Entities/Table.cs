using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  TABLE / QR
// =====================================
public class Table : SoftDeletableEntity<Guid>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public Guid? AreaId { get; private set; }
    public string Code { get; private set; } = null!;          // B01...
    public string QrCodeToken { get; private set; } = null!;   // unique
    public int? Seats { get; private set; }
    public Guid? StatusId { get; private set; }
    public bool DoNotAllowOrder { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual TableArea? Area { get; private set; }
    public virtual CodeItem? Status { get; private set; }
    public virtual ICollection<Order> Orders { get; private set; }
}
