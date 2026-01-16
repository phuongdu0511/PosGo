using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishVariant;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.DishVariant;

public sealed class CreateVariantOptionCommandHandler : ICommandHandler<Command.CreateVariantOptionCommand>
{
    private readonly IRepositoryBase<DishVariantOption, int> _optionRepository;
    private readonly IRepositoryBase<Domain.Entities.DishVariant, int> _variantRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateVariantOptionCommandHandler(
        IRepositoryBase<DishVariantOption, int> optionRepository,
        IRepositoryBase<Domain.Entities.DishVariant, int> variantRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _optionRepository = optionRepository;
        _variantRepository = variantRepository;
        _languageRepository = languageRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.CreateVariantOptionCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Validate Variant exists
        var variant = await _variantRepository.FindByIdAsync(request.VariantId, cancellationToken);
        if (variant == null)
        {
            return Result.Failure(new Error("VARIANT_NOT_FOUND", "Không tìm thấy biến thể."));
        }

        // Check duplicate Code for this variant
        var existingOption = await _optionRepository.FindSingleAsync(
            x => x.VariantId == request.VariantId && x.Code == request.Code.Trim(), cancellationToken);

        if (existingOption != null)
        {
            return Result.Failure(new Error("OPTION_CODE_EXISTS", "Mã tùy chọn đã tồn tại cho biến thể này."));
        }

        // If setting as default, unset other defaults
        if (request.IsDefault)
        {
            var currentDefaults = await _optionRepository.FindAll(x => x.VariantId == request.VariantId && x.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var defaultOption in currentDefaults)
            {
                defaultOption.UnsetDefault();
            }
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

        // Create Option
        var option = DishVariantOption.Create(
            restaurantId.Value,
            request.VariantId,
            request.Code,
            request.SortOrder,
            request.IsDefault,
            request.PriceAdjustment,
            request.IsActive);

        _optionRepository.Add(option);

        return Result.Success();
    }
}