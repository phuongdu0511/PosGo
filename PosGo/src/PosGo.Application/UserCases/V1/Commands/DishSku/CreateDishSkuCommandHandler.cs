using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishSku;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.DishSku;

public sealed class CreateDishSkuCommandHandler : ICommandHandler<Command.CreateDishSkuCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IRepositoryBase<DishVariantOption, int> _optionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDishSkuCommandHandler(
        IRepositoryBase<Domain.Entities.DishSku, int> skuRepository,
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IRepositoryBase<DishVariantOption, int> optionRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _skuRepository = skuRepository;
        _dishRepository = dishRepository;
        _optionRepository = optionRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.CreateDishSkuCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Validate Dish exists
        var dish = await _dishRepository.FindByIdAsync(request.DishId, cancellationToken);
        if (dish == null)
        {
            return Result.Failure(new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        // Check duplicate Code for this dish
        var existingSku = await _skuRepository.FindSingleAsync(
            x => x.DishId == request.DishId && x.Code == request.Code.Trim(), cancellationToken);

        if (existingSku != null)
        {
            return Result.Failure(new Error("SKU_CODE_EXISTS", "Mã SKU đã tồn tại cho món ăn này."));
        }

        // Validate Variant Options exist
        if (request.VariantOptionIds.Any())
        {
            var existingOptionIds = await _optionRepository.FindAll(o => request.VariantOptionIds.Contains(o.Id))
                .Select(o => o.Id)
                .ToListAsync(cancellationToken);

            var missingOptionIds = request.VariantOptionIds.Except(existingOptionIds).ToList();
            if (missingOptionIds.Any())
            {
                return Result.Failure(new Error("VARIANT_OPTION_NOT_FOUND", 
                    $"Không tìm thấy tùy chọn biến thể với ID: {string.Join(", ", missingOptionIds)}"));
            }
        }

        // If setting as default, unset other defaults
        if (request.IsDefault)
        {
            var currentDefaults = await _skuRepository.FindAll(x => x.DishId == request.DishId && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var defaultSku in currentDefaults)
            {
                defaultSku.UnsetDefault();
            }
        }

        // Create SKU
        var sku = Domain.Entities.DishSku.Create(
            restaurantId.Value,
            request.DishId,
            request.Code,
            request.Price,
            request.IsDefault,
            request.StockQuantity,
            request.ImageUrl,
            request.IsActive,
            request.CostPrice);

        _skuRepository.Add(sku);

        return Result.Success();
    }
}