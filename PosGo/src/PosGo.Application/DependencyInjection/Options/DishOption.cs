namespace PosGo.Application.DependencyInjection.Options;

public class DishOption
{
    /// <summary>
    /// Maximum number of images allowed per dish
    /// </summary>
    public int MaxDishImages { get; set; }
    /// <summary>
    /// Maximum number of SKU combinations when generating variants
    /// </summary>
    public int MaxSkuCombinations { get; set; }
    /// <summary>
    /// Maximum number of variants
    /// </summary>
    public int MaxVariants { get; set; }

    /// <summary>
    /// Maximum number of options each variant
    /// </summary>
    public int MaxOptions { get; set; }
}
