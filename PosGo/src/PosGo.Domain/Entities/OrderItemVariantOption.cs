namespace PosGo.Domain.Entities;

public class OrderItemVariantOption
{
    public int OrderItemId { get; private set; }
    public int VariantOptionId { get; private set; }

    // Snapshot data (optional - để lưu tên khi order, phòng khi variant option bị xóa)
    public string? VariantName { get; private set; }  // "Size"
    public string? OptionName { get; private set; }  // "M"
    public decimal? PriceAdjustment { get; private set; }  // Snapshot của PriceAdjustment khi order

    public virtual OrderItem OrderItem { get; private set; } = null!;
    public virtual DishVariantOption VariantOption { get; private set; } = null!;
}
