using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishVariant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.DishVariant;

public sealed class CreateDishVariantCommandHandler : ICommandHandler<Command.CreateDishVariantCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishVariant, int> _variantRepository;
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDishVariantCommandHandler(
        IRepositoryBase<Domain.Entities.DishVariant, int> variantRepository,
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _variantRepository = variantRepository;
        _dishRepository = dishRepository;
        _languageRepository = languageRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.CreateDishVariantCommand request, CancellationToken cancellationToken)
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
        var existingVariant = await _variantRepository.FindSingleAsync(
            x => x.DishId == request.DishId && x.Code == request.Code.Trim(), cancellationToken);

        if (existingVariant != null)
        {
            return Result.Failure(new Error("VARIANT_CODE_EXISTS", "Mã biến thể đã tồn tại cho món ăn này."));
        }

        // Validate Languages
        var languageIds = request.Translations.Select(t => t.LanguageId).Distinct().ToList();
        var existingLanguageIds = await _languageRepository.FindAll(l => languageIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        var missingLanguageIds = languageIds.Except(existingLanguageIds).ToList();
        if (missingLanguageIds.Any())
        {
            return Result.Failure(new Error("LANGUAGE_NOT_FOUND", 
                $"Không tìm thấy ngôn ngữ với ID: {string.Join(", ", missingLanguageIds)}"));
        }

        // Create Variant
        var variant = Domain.Entities.DishVariant.Create(
            restaurantId.Value,
            request.DishId,
            request.Code,
            request.SortOrder,
            request.IsActive);

        _variantRepository.Add(variant);

        return Result.Success();
    }
}