using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.DishCategory;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Commands.DishCategory;

public sealed class UpdateDishCategoryCommandHandler : ICommandHandler<Command.UpdateDishCategoryCommand>
{
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IRepositoryBase<DishCategoryTranslation, int> _translationRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;

    public UpdateDishCategoryCommandHandler(
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IRepositoryBase<DishCategoryTranslation, int> translationRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository)
    {
        _categoryRepository = categoryRepository;
        _translationRepository = translationRepository;
        _languageRepository = languageRepository;
    }

    public async Task<Result> Handle(Command.UpdateDishCategoryCommand request, CancellationToken cancellationToken)
    {
        // Find Category
        var category = await _categoryRepository.FindByIdAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result.Failure(new Error("CATEGORY_NOT_FOUND", "Không tìm thấy danh mục."));
        }

        // Validate Name is unique in Restaurant (exclude current category)
        var existingCategory = await _categoryRepository.FindSingleAsync(
            c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower() && 
                 c.Id != request.Id,
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

            // Prevent circular reference
            if (request.ParentCategoryId.Value == request.Id)
            {
                return Result.Failure(new Error("CIRCULAR_REFERENCE", "Danh mục không thể là cha của chính nó."));
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

        // Update Category with Name and Description
        category.Update(
            request.Name,
            request.Description,
            request.ParentCategoryId,
            request.ImageUrl,
            request.SortOrder,
            request.IsActive,
            request.ShowOnMenu);

        // Delete all existing translations
        var existingTranslations = await _translationRepository
            .FindAll(t => t.CategoryId == request.Id)
            .ToListAsync(cancellationToken);

        if (existingTranslations.Any())
        {
            _translationRepository.RemoveMultiple(existingTranslations);
        }

        // Add new translations (if provided)
        if (request.Translations != null && request.Translations.Any())
        {
            foreach (var translationDto in request.Translations)
            {
                var translation = DishCategoryTranslation.Create(
                    request.Id,
                    translationDto.LanguageId,
                    translationDto.Name,
                    translationDto.Description);

                _translationRepository.Add(translation);
            }
        }

        return Result.Success();
    }
}