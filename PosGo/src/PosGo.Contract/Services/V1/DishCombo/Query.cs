using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.DishCombo.Response;

namespace PosGo.Contract.Services.V1.DishCombo;

public static class Query
{
    public record GetComboItemsQuery(int ComboDishId, string? Language = "vi") : IQuery<List<ComboItemResponse>>;

    public record GetComboDetailQuery(int ComboDishId, string? Language = "vi") : IQuery<ComboDetailResponse>;
}