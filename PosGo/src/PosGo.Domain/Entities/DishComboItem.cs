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
}
