using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishSku : AuditableEntity<int>, ITenantEntity
{
    public Guid RestaurantId { get; private set; }
    public int DishId { get; private set; }
    public string Sku { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? CostPrice { get; private set; }
    public virtual Restaurant Restaurant { get; private set; } = null!;
    public virtual Dish Dish { get; private set; } = null!;
    public virtual ICollection<DishSkuVariantOption> VariantOptions { get; private set; }
    public virtual ICollection<OrderItem> OrderItems { get; private set; }

    // Private constructor
    public DishSku(Guid restaurantId, int dishId, string sku, decimal price, int stockQuantity, string? imageUrl, bool isActive, decimal? costPrice)
    {
        RestaurantId = restaurantId;
        DishId = dishId;
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl?.Trim();
        IsActive = isActive;
        CostPrice = costPrice;
    }

    // Factory method
    public static DishSku Create(
        Guid restaurantId,
        Dish dish,
        string sku,
        decimal price,
        int stockQuantity = 0,
        string? imageUrl = null,
        bool isActive = true,
        decimal? costPrice = null)
    {
        var dishSku = new DishSku(restaurantId, dish.Id, sku, price, stockQuantity, imageUrl, isActive, costPrice);

        // Set navigation property
        dishSku.Dish = dish;

        return dishSku;
    }

    // Business methods
    public void Update(string sku, decimal price, int stockQuantity, string? imageUrl, bool isActive, decimal? costPrice)
    {
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        ImageUrl = imageUrl?.Trim();
        IsActive = isActive;
        CostPrice = costPrice;
    }

    public void UpdatePrice(decimal price) => Price = price;
    public void UpdateStock(int stockQuantity) => StockQuantity = stockQuantity;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
