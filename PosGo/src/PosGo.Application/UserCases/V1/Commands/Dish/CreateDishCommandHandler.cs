using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Dish;

public sealed class CreateDishCommandHandler : ICommandHandler<Command.CreateDishCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IRepositoryBase<DishTranslation, int> _translationRepository;
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IRepositoryBase<Unit, int> _unitRepository;
    private readonly IRepositoryBase<CodeItem, int> _codeItemRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CreateDishCommandHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IRepositoryBase<DishTranslation, int> translationRepository,
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IRepositoryBase<Unit, int> unitRepository,
        IRepositoryBase<CodeItem, int> codeItemRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _dishRepository = dishRepository;
        _translationRepository = translationRepository;
        _categoryRepository = categoryRepository;
        _unitRepository = unitRepository;
        _codeItemRepository = codeItemRepository;
        _languageRepository = languageRepository;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateDishCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Validate Category (if provided)
        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.FindByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result.Failure(new Error("CATEGORY_NOT_FOUND", "Không tìm thấy danh mục."));
            }
        }

        // Validate Unit (if provided)
        if (request.UnitId.HasValue)
        {
            var unit = await _unitRepository.FindByIdAsync(request.UnitId.Value, cancellationToken);
            if (unit == null)
            {
                return Result.Failure(new Error("UNIT_NOT_FOUND", "Không tìm thấy đơn vị tính."));
            }
        }

        // Validate DishType (if provided)
        if (request.DishTypeId.HasValue)
        {
            var dishType = await _codeItemRepository.FindByIdAsync(request.DishTypeId.Value, cancellationToken);
            if (dishType == null)
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

        // Create Dish
        var dish = Domain.Entities.Dish.Create(
            restaurantId.Value,
            request.Name,
            request.Description,
            request.CategoryId,
            request.UnitId,
            request.DishTypeId,
            request.SortOrder,
            request.IsActive,
            request.IsAvailable,
            request.ShowOnMenu,
            request.TaxRate);

        _dishRepository.Add(dish);

        // Create Translations using navigation property
        foreach (var translationDto in request.Translations)
        {
            var translation = DishTranslation.Create(
                dish,
                translationDto.LanguageId,
                translationDto.Name,
                translationDto.Description);
            
            _translationRepository.Add(translation);
        }

        return Result.Success();
    }
}