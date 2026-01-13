using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  ORDER / REPORT
// =====================================
public class OrderItem : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int OrderId { get; private set; }

    public int? DishId { get; private set; }
    public int? DishSkuId { get; private set; }

    public string DishName { get; private set; } = null!;    // snapshot
    public string? DishUnit { get; private set; }           // snapshot
    public string? SkuCode { get; private set; }            // snapshot
    public string? SkuLabel { get; private set; }           // snapshot

    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    public string? Note { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Order Order { get; private set; } = null!;
    public virtual Dish? Dish { get; private set; }
    public virtual DishSku? DishSku { get; private set; }
    public virtual ICollection<OrderItemVariantOption> VariantOptions { get; private set; }
}
