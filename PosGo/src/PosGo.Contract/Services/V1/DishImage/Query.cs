using PosGo.Contract.Abstractions.Shared;
using static PosGo.Contract.Services.V1.DishImage.Response;

namespace PosGo.Contract.Services.V1.DishImage;

public static class Query
{
    public record GetDishImagesQuery(int DishId) : IQuery<List<DishImageResponse>>;

    public record GetPrimaryImageQuery(int DishId) : IQuery<DishImageResponse>;
}