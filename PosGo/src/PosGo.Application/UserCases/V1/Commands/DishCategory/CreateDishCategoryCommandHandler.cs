using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.DishCategory;

public sealed class CreateDishCategoryCommandHandler : ICommandHandler<Command.CreateDishCategoryCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IRepositoryBase<Domain.Entities.DishCategoryTranslation, int> _translationRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDishCategoryCommandHandler(
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IRepositoryBase<Domain.Entities.DishCategoryTranslation, int> translationRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _categoryRepository = categoryRepository;
        _translationRepository = translationRepository;
        _languageRepository = languageRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.CreateDishCategoryCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var restaurantId = httpContext.GetRestaurantId();

        if (!restaurantId.HasValue)
        {
            return Result.Failure(new Error("RESTAURANT_NOT_FOUND", "Không tìm thấy thông tin nhà hàng."));
        }

        // Validate Name is unique in Restaurant
        var existingCategory = await _categoryRepository.FindSingleAsync(
            c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower(),
            cancellationToken);

        if (existingCategory != null)
        {
            return Result.Failure(new Error("CATEGORY_NAME_EXISTS", "Tên danh mục đã tồn tại."));
        }

        // Validate ParentCategory (if provided)
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.FindByIdAsync(request.ParentCategoryId.Value, cancellationToken);
            if (parentCategory == null)
            {
                return Result.Failure(new Error("PARENT_CATEGORY_NOT_FOUND", "Không tìm thấy danh mục cha."));
            }
        }

        // Validate Languages (if translations provided)
        if (request.Translations != null && request.Translations.Any())
        {
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

        // Create Category with default Name and Description
        var category = Domain.Entities.DishCategory.Create(
            restaurantId.Value,
            request.Name,
            request.Description,
            request.ParentCategoryId,
            request.ImageUrl,
            request.SortOrder,
            request.IsActive,
            request.ShowOnMenu);

        _categoryRepository.Add(category);

        // Create Translations (optional)
        if (request.Translations != null && request.Translations.Any())
        {
            foreach (var translationDto in request.Translations)
            {
                var translation = Domain.Entities.DishCategoryTranslation.Create(
                    category,
                    translationDto.LanguageId,
                    translationDto.Name,
                    translationDto.Description);

                _translationRepository.Add(translation);
            }
        }

        return Result.Success();
    }
}