using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishSku : SoftDeletableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int DishId { get; private set; }

    public string Code { get; private set; } = null!;
    public decimal Price { get; private set; }
    public bool IsDefault { get; private set; }
    public int StockQuantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? CostPrice { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishSkuVariantOption> VariantOptions { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }
}
