using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class DishComboItem : Entity<int>
{
    public int ComboDishId { get; private set; }  // Dish cha (combo)
    public int ItemDishId { get; private set; }   // Dish con (món trong combo)
    public int? ItemDishSkuId { get; private set; }  // SKU cụ thể (nếu có)
    public int Quantity { get; private set; }  // Số lượng
    public bool IsRequired { get; private set; }  // Bắt buộc hay tùy chọn
    public int DisplayOrder { get; private set; }

    // Private constructor
    private DishComboItem(int comboDishId, int itemDishId, int quantity, bool isRequired, int displayOrder, int? itemDishSkuId = null)
    {
        ComboDishId = comboDishId;
        ItemDishId = itemDishId;
        Quantity = quantity;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        ItemDishSkuId = itemDishSkuId;
    }

    // Factory method
    public static DishComboItem Create(int comboDishId, int itemDishId, int quantity, bool isRequired = false, int displayOrder = 0, int? itemDishSkuId = null)
        => new DishComboItem(comboDishId, itemDishId, quantity, isRequired, displayOrder, itemDishSkuId);

    // Business methods
    public void Update(int quantity, bool isRequired, int displayOrder, int? itemDishSkuId = null)
    {
        Quantity = quantity;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
        ItemDishSkuId = itemDishSkuId;
    }
}
