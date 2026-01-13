using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

// =====================================
//  VARIANTS + SKU
// =====================================
public class DishSkuVariantOption
{
    public int DishSkuId { get; private set; }
    public int VariantOptionId { get; private set; }

    public virtual DishSku DishSku { get; private set; } = null!;
    public virtual DishVariantOption VariantOption { get; private set; } = null!;
}
