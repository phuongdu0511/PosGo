using PosGo.Contract.Abstractions.Shared;

namespace PosGo.Contract.Services.V1.DishImage;

public static class Command
{
    public record UploadDishImageCommand(
        int DishId,
        string ImageUrl,
        int DisplayOrder,
        bool IsPrimary,
        string? AltText
    ) : ICommand;

    public record UpdateDishImageCommand(
        int Id,
        string ImageUrl,
        int DisplayOrder,
        bool IsPrimary,
        string? AltText
    ) : ICommand;

    public record DeleteDishImageCommand(int Id) : ICommand;

    public record SetPrimaryImageCommand(int Id) : ICommand;

    public record ReorderDishImagesCommand(
        int DishId,
        List<ImageOrderDto> Images
    ) : ICommand;
}

public record ImageOrderDto(
    int Id,
    int DisplayOrder
);