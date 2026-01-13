using PosGo.Domain.Abstractions.Entities;

namespace PosGo.Domain.Entities;

public class DishImage : Entity<int>
{
    public int DishId { get; private set; }
    public string ImageUrl { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? AltText { get; private set; }
}
