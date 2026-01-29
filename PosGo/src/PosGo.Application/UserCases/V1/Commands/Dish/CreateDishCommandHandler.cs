using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PosGo.Application.DependencyInjection.Options;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Commands.Dish;

public sealed class CreateDishCommandHandler : ICommandHandler<Command.CreateDishCommand>
{
    private readonly DishOption dishOption = new DishOption();

    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IRepositoryBase<DishTranslation, int> _translationRepository;
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IRepositoryBase<Unit, int> _unitRepository;
    private readonly IRepositoryBase<CodeItem, int> _codeItemRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IRepositoryBase<Domain.Entities.DishVariant, int> _variantRepository;
    private readonly IRepositoryBase<DishVariantOption, int> _variantOptionRepository;
    private readonly IRepositoryBase<Domain.Entities.DishSku, int> _skuRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public CreateDishCommandHandler(
        IConfiguration configuration,
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IRepositoryBase<DishTranslation, int> translationRepository,
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IRepositoryBase<Unit, int> unitRepository,
        IRepositoryBase<CodeItem, int> codeItemRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IRepositoryBase<Domain.Entities.DishVariant, int> variantRepository,
        IRepositoryBase<DishVariantOption, int> variantOptionRepository,
        IRepositoryBase<Domain.Entities.DishSku, int> skuRepository,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context)
    {
        configuration.GetSection(nameof(DishOption)).Bind(dishOption);
        _dishRepository = dishRepository;
        _translationRepository = translationRepository;
        _categoryRepository = categoryRepository;
        _unitRepository = unitRepository;
        _codeItemRepository = codeItemRepository;
        _languageRepository = languageRepository;
        _variantRepository = variantRepository;
        _variantOptionRepository = variantOptionRepository;
        _skuRepository = skuRepository;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<Result> Handle(Command.CreateDishCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Validate input
        var validationResult = await ValidateDishInput(request, cancellationToken);
        if (validationResult.IsFailure)
            return validationResult;

        // Validate variants
        var variantValidationResult = ValidateVariants(request.Variants);
        if (variantValidationResult.IsFailure)
            return variantValidationResult;

        // Create Dish entity
        var dish = CreateDishEntity(restaurantId.Value, request);
        _dishRepository.Add(dish);

        // Create Translations
        if (request.Translations != null && request.Translations.Any())
        {
            CreateTranslations(dish, request.Translations);
        }

        // Handle Variants & SKUs
        try
        {
            if (request.Variants == null || !request.Variants.Any())
            {
                // CASE 1: Dish không có variant → Tạo 1 SKU mặc định
                CreateDefaultSku(restaurantId.Value, dish, request);
            }
            else
            {
                // CASE 2: Dish có variant → Tạo Variants + Options + SKUs
                var createdVariants = await CreateVariantsAndOptions(restaurantId.Value, dish, request.Variants, cancellationToken);

                if (request.GenerateSkus)
                {
                    await GenerateSkuCombinations(restaurantId.Value, dish, createdVariants, request.BasePrice, cancellationToken);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("SKU_COMBINATIONS_EXCEEDED", ex.Message));
        }

        return Result.Success();
    }

    private async Task<Result> ValidateDishInput(Command.CreateDishCommand request, CancellationToken cancellationToken)
    {
        // Validate BasePrice
        if (request.BasePrice <= 0)
        {
            return Result.Failure(new Error("INVALID_BASE_PRICE", "Giá cơ bản phải lớn hơn 0."));
        }

        // Validate Images (max 5)
        if (request.ImageKeys != null && request.ImageKeys.Count > dishOption.MaxDishImages)
        {
            return Result.Failure(new Error("TOO_MANY_IMAGES", $"Chỉ được phép tối đa {dishOption.MaxDishImages} ảnh."));
        }

        // Validate Category (if provided)
        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.FindAll(c => c.Id == request.CategoryId.Value).AnyAsync(cancellationToken);
            if (!category)
            {
                return Result.Failure(new Error("CATEGORY_NOT_FOUND", "Không tìm thấy danh mục."));
            }
        }

        // Validate Unit (if provided)
        if (request.UnitId.HasValue)
        {
            var unit = await _unitRepository.FindAll(c => c.Id == request.UnitId.Value).AnyAsync(cancellationToken);
            if (!unit)
            {
                return Result.Failure(new Error("UNIT_NOT_FOUND", "Không tìm thấy đơn vị tính."));
            }
        }

        // Validate DishType (if provided)
        if (request.DishTypeId.HasValue)
        {
            var dishType = await _codeItemRepository.FindAll(c => c.Id == request.DishTypeId.Value).AnyAsync(cancellationToken);
            if (!dishType)
            {
                return Result.Failure(new Error("DISH_TYPE_NOT_FOUND", "Không tìm thấy loại món ăn."));
            }
        }

        // Validate Languages (if translations provided)
        if (request.Translations != null && request.Translations.Any())
        {
            // Check duplicate LanguageId in request
            var duplicateLanguages = request.Translations
                .GroupBy(t => t.LanguageId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateLanguages.Any())
            {
                return Result.Failure(new Error("DUPLICATE_LANGUAGE",
                    $"Ngôn ngữ bị trùng lặp: {string.Join(", ", duplicateLanguages)}"));
            }

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
        }

        return Result.Success();
    }

    private Result ValidateVariants(List<DishVariantDto>? variants)
    {
        if (variants == null || !variants.Any())
            return Result.Success();

        if (variants.Count > dishOption.MaxVariants)
        {
            return Result.Failure(new Error("TOO_MANY_VARIANTS", $"Chỉ được phép tối đa {dishOption.MaxVariants} biến thể."));
        }

        // Check duplicate Variant Codes
        var variantCodes = variants.Select(v => v.Code.Trim().ToUpperInvariant()).ToList();
        var duplicateCodes = variantCodes.GroupBy(c => c).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicateCodes.Any())
        {
            return Result.Failure(new Error("DUPLICATE_VARIANT_CODE",
                $"Mã biến thể bị trùng lặp: {string.Join(", ", duplicateCodes)}"));
        }

        // Validate each Variant has at least 1 Option
        foreach (var variant in variants)
        {
            if (variant.Options == null || !variant.Options.Any())
            {
                return Result.Failure(new Error("VARIANT_MUST_HAVE_OPTIONS",
                    $"Biến thể '{variant.Code}' phải có ít nhất 1 tùy chọn."));
            }

            if (variant.Options.Count > dishOption.MaxOptions)
            {
                return Result.Failure(new Error("TOO_MANY_OPTIONS",
                    $"Biến thể '{variant.Code}' chỉ được phép tối đa {dishOption.MaxOptions} giá trị."));
            }

            // Check duplicate Option Codes within Variant
            var optionCodes = variant.Options.Select(o => o.Code.Trim().ToUpperInvariant()).ToList();
            var duplicateOptionCodes = optionCodes.GroupBy(c => c).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateOptionCodes.Any())
            {
                return Result.Failure(new Error("DUPLICATE_OPTION_CODE",
                    $"Mã tùy chọn bị trùng lặp trong biến thể '{variant.Code}': {string.Join(", ", duplicateOptionCodes)}"));
            }
        }

        return Result.Success();
    }

    private Domain.Entities.Dish CreateDishEntity(Guid restaurantId, Command.CreateDishCommand request)
    {
        // Images đã được validate trong ValidateDishInput, không cần Take() nữa
        var imageKeys = request.ImageKeys?.ToArray();

        return Domain.Entities.Dish.Create(
            restaurantId,
            request.Name,
            request.Description,
            request.CategoryId,
            request.UnitId,
            request.DishTypeId,
            request.SortOrder,
            request.IsActive,
            request.IsAvailable,
            request.ShowOnMenu,
            request.TaxRate,
            imageKeys);
    }

    private void CreateTranslations(Domain.Entities.Dish dish, List<DishTranslationDto> translations)
    {
        var translationEntities = translations.Select(dto => DishTranslation.Create(
            dish,
            dto.LanguageId,
            dto.Name,
            dto.Description)).ToList();

        _translationRepository.AddRange(translationEntities);
    }

    private void CreateDefaultSku(Guid restaurantId, Domain.Entities.Dish dish, Command.CreateDishCommand request)
    {
        var defaultSkuCode = $"{request.Name.Trim().Replace(" ", "-").ToUpperInvariant()}-DEFAULT";
        var defaultSku = Domain.Entities.DishSku.Create(
            restaurantId,
            dish,
            sku: defaultSkuCode,
            price: request.BasePrice,
            stockQuantity: 0,
            imageUrl: null,
            isActive: true,
            costPrice: null
        );
        _skuRepository.Add(defaultSku);
    }

    private async Task<List<Domain.Entities.DishVariant>> CreateVariantsAndOptions(
        Guid restaurantId,
        Domain.Entities.Dish dish,
        List<DishVariantDto> variantDtos,
        CancellationToken cancellationToken)
    {
        var createdVariants = new List<Domain.Entities.DishVariant>();

        // Tạo tất cả variants trước
        foreach (var variantDto in variantDtos)
        {
            var variant = Domain.Entities.DishVariant.Create(
                restaurantId,
                dish,
                variantDto.Code,
                variantDto.SortOrder,
                variantDto.IsActive
            );
            createdVariants.Add(variant);
        }

        // Add all variants at once
        _variantRepository.AddRange(createdVariants);

        // Tạo tất cả options sau khi variants đã có Id
        var allOptions = new List<DishVariantOption>();
        foreach (var (variant, variantDto) in createdVariants.Zip(variantDtos))
        {
            foreach (var optionDto in variantDto.Options)
            {
                var option = DishVariantOption.Create(
                    restaurantId,
                    variant.Id,
                    optionDto.Code,
                    optionDto.SortOrder,
                    optionDto.PriceAdjustment,
                    optionDto.IsActive
                );
                variant.Options.Add(option);
                allOptions.Add(option);
            }
        }

        _variantOptionRepository.AddRange(allOptions);

        return createdVariants;
    }

    private async Task GenerateSkuCombinations(
        Guid restaurantId,
        Domain.Entities.Dish dish,
        List<Domain.Entities.DishVariant> variants,
        decimal basePrice,
        CancellationToken cancellationToken)
    {
        // Generate combinations using iterative approach (không dùng recursive)
        var combinations = GenerateVariantCombinationsIterative(variants);

        // Throw lỗi nếu vượt quá giới hạn
        if (combinations.Count > dishOption.MaxSkuCombinations)
        {
            var totalOptions = variants.Sum(v => v.Options.Count(o => o.IsActive));
            throw new InvalidOperationException(
                $"Số lượng SKU combinations ({combinations.Count}) vượt quá giới hạn cho phép ({dishOption.MaxSkuCombinations}). " +
                $"Vui lòng giảm số lượng variants hoặc options. " +
                $"(Hiện tại: {variants.Count} variants × trung bình {totalOptions / variants.Count} options/variant)");
        }

        var skus = new List<Domain.Entities.DishSku>();
        var skuVariantOptions = new List<DishSkuVariantOption>();

        foreach (var combination in combinations)
        {
            // Tính giá: BasePrice + tổng PriceAdjustment của các options
            var totalPriceAdjustment = combination.Options.Sum(o => o.PriceAdjustment);
            var skuPrice = basePrice + totalPriceAdjustment;

            // Tạo SKU Code
            var skuCode = GenerateSkuCode(combination.Options);

            var sku = Domain.Entities.DishSku.Create(
                restaurantId,
                dish,
                sku: skuCode,
                price: skuPrice,
                stockQuantity: 0,
                imageUrl: null,
                isActive: true,
                costPrice: null
            );
            skus.Add(sku);
        }

        // Add all SKUs at once
        _skuRepository.AddRange(skus);

        // Tạo DishSkuVariantOption (junction) cho mỗi SKU
        foreach (var (sku, combination) in skus.Zip(combinations))
        {
            foreach (var option in combination.Options)
            {
                var skuVariantOption = DishSkuVariantOption.Create(sku, option);
                skuVariantOptions.Add(skuVariantOption);
            }
        }

        _context.DishSkuVariantOptions.AddRange(skuVariantOptions);
    }

    // Iterative approach thay vì recursive để tránh stack overflow
    private List<VariantCombination> GenerateVariantCombinationsIterative(List<Domain.Entities.DishVariant> variants)
    {
        if (!variants.Any())
            return new List<VariantCombination>();

        // Tính tổng số combinations có thể có (ước lượng)
        var totalPossibleCombinations = variants
            .Select(v => v.Options.Count(o => o.IsActive))
            .Aggregate(1L, (acc, count) => acc * count);

        // Throw lỗi sớm nếu ước lượng vượt quá giới hạn
        if (totalPossibleCombinations > dishOption.MaxSkuCombinations)
        {
            var variantDetails = string.Join(", ", variants.Select(v =>
                $"{v.Name}({v.Options.Count(o => o.IsActive)} options)"));
            throw new InvalidOperationException(
                $"Số lượng SKU combinations ước tính ({totalPossibleCombinations}) vượt quá giới hạn cho phép ({dishOption.MaxSkuCombinations}). " +
                $"Vui lòng giảm số lượng variants hoặc options. " +
                $"(Variants: {variantDetails})");
        }

        var result = new List<VariantCombination>();
        var stack = new Stack<List<DishVariantOption>>();

        // Initialize với empty list
        stack.Push(new List<DishVariantOption>());

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            // Nếu đã chọn đủ options cho tất cả variants
            if (current.Count == variants.Count)
            {
                result.Add(new VariantCombination
                {
                    Options = current,
                });

                // Check sau mỗi combination được tạo
                if (result.Count > dishOption.MaxSkuCombinations)
                {
                    throw new InvalidOperationException(
                        $"Số lượng SKU combinations ({result.Count}) vượt quá giới hạn cho phép ({dishOption.MaxSkuCombinations}). " +
                        $"Vui lòng giảm số lượng variants hoặc options.");
                }
                continue;
            }

            // Lấy variant tiếp theo
            var variantIndex = current.Count;
            var variant = variants[variantIndex];
            var activeOptions = variant.Options.Where(o => o.IsActive).ToList();

            // Thêm mỗi option vào stack
            foreach (var option in activeOptions)
            {
                var next = new List<DishVariantOption>(current) { option };
                stack.Push(next);
            }

            // Check để tránh quá nhiều combinations trong stack
            if (result.Count + stack.Count > dishOption.MaxSkuCombinations)
            {
                throw new InvalidOperationException(
                    $"Số lượng SKU combinations ước tính ({result.Count + stack.Count}) vượt quá giới hạn cho phép ({dishOption.MaxSkuCombinations}). " +
                    $"Vui lòng giảm số lượng variants hoặc options.");
            }
        }

        return result;
    }

    private string GenerateSkuCode(List<DishVariantOption> options)
    {
        var optionCodes = string.Join("-", options.Select(o => o.Value));
        return optionCodes;
    }

    private class VariantCombination
    {
        public List<DishVariantOption> Options { get; set; } = new();
    }
}
