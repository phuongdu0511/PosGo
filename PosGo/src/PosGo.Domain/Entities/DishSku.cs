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

    // Private constructor
    private DishSku(Guid restaurantId, int dishId, string code, decimal price, bool isDefault, int stockQuantity, string? imageUrl, bool isActive, decimal? costPrice)
    {
        RestaurantId = restaurantId;
        DishId = dishId;
        Code = code.Trim();
        Price = price;
        IsDefault = isDefault;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl?.Trim();
        IsActive = isActive;
        CostPrice = costPrice;
    }

    // Factory method
    public static DishSku Create(Guid restaurantId, int dishId, string code, decimal price, bool isDefault = false, int stockQuantity = 0, string? imageUrl = null, bool isActive = true, decimal? costPrice = null)
        => new DishSku(restaurantId, dishId, code, price, isDefault, stockQuantity, imageUrl, isActive, costPrice);

    // Business methods
    public void Update(string code, decimal price, bool isDefault, int stockQuantity, string? imageUrl, bool isActive, decimal? costPrice)
    {
        Code = code.Trim();
        Price = price;
        IsDefault = isDefault;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl?.Trim();
        IsActive = isActive;
        CostPrice = costPrice;
    }

    public void UpdatePrice(decimal price) => Price = price;
    public void UpdateStock(int stockQuantity) => StockQuantity = stockQuantity;
    public void SetAsDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
