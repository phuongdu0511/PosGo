using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Dish;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;

namespace PosGo.Application.UserCases.V1.Commands.Dish;

public sealed class UpdateDishCommandHandler : ICommandHandler<Command.UpdateDishCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Dish, int> _dishRepository;
    private readonly IRepositoryBase<DishTranslation, int> _translationRepository;
    private readonly IRepositoryBase<Domain.Entities.DishCategory, int> _categoryRepository;
    private readonly IRepositoryBase<Unit, int> _unitRepository;
    private readonly IRepositoryBase<CodeItem, int> _codeItemRepository;
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;

    public UpdateDishCommandHandler(
        IRepositoryBase<Domain.Entities.Dish, int> dishRepository,
        IRepositoryBase<DishTranslation, int> translationRepository,
        IRepositoryBase<Domain.Entities.DishCategory, int> categoryRepository,
        IRepositoryBase<Unit, int> unitRepository,
        IRepositoryBase<CodeItem, int> codeItemRepository,
        IRepositoryBase<Domain.Entities.Language, int> languageRepository)
    {
        _dishRepository = dishRepository;
        _translationRepository = translationRepository;
        _categoryRepository = categoryRepository;
        _unitRepository = unitRepository;
        _codeItemRepository = codeItemRepository;
        _languageRepository = languageRepository;
    }

    public async Task<Result> Handle(Command.UpdateDishCommand request, CancellationToken cancellationToken)
    {
        // Find Dish with Translations
        var dish = await _dishRepository.FindByIdAsync(request.Id, cancellationToken, d => d.Translations);
        if (dish == null)
        {
            return Result.Failure(new Error("DISH_NOT_FOUND", "Không tìm thấy món ăn."));
        }

        // Validate Translations is not empty
        if (request.Translations == null || !request.Translations.Any())
        {
            return Result.Failure(new Error("TRANSLATIONS_REQUIRED", "Bản dịch là bắt buộc."));
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

        // Validate Languages
        var languageIds = request.Translations.Select(t => t.LanguageId).Distinct().ToList();
        var languages = await _languageRepository.FindAll(l => languageIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        var missingLanguageIds = languageIds.Except(languages.Select(l => l.Id)).ToList();
        if (missingLanguageIds.Any())
        {
            return Result.Failure(new Error("LANGUAGE_NOT_FOUND", 
                $"Không tìm thấy ngôn ngữ với ID: {string.Join(", ", missingLanguageIds)}"));
        }

        // Check duplicate Name in Translation table for each language (excluding current dish)
        foreach (var translation in request.Translations)
        {
            var duplicateName = await _translationRepository.FindSingleAsync(
                t => t.DishId != request.Id &&
                     t.LanguageId == translation.LanguageId && 
                     t.Name.Trim().ToLower() == translation.Name.Trim().ToLower(),
                cancellationToken);

            if (duplicateName != null)
            {
                return Result.Failure(new Error("DISH_NAME_EXISTS", 
                    $"Tên món ăn '{translation.Name}' đã tồn tại cho ngôn ngữ này."));
            }
        }

        // Update Dish
        dish.Update(
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

        // Update Translations
        // Get existing translations
        var existingTranslations = await _translationRepository
            .FindAll(t => t.DishId == request.Id, t => t.Language)
            .ToListAsync(cancellationToken);

        // Remove translations that are not in the request
        var requestLanguageIds = request.Translations.Select(t => t.LanguageId).ToList();
        var translationsToRemove = existingTranslations
            .Where(t => !requestLanguageIds.Contains(t.LanguageId))
            .ToList();

        if (translationsToRemove.Any())
        {
            _translationRepository.RemoveMultiple(translationsToRemove);
        }

        foreach (var translationDto in request.Translations)
        {
            var existingTranslation = existingTranslations
                .FirstOrDefault(t => t.LanguageId == translationDto.LanguageId);

            if (existingTranslation != null)
            {
                // Update existing translation
                existingTranslation.Update(translationDto.Name, translationDto.Description);
            }
            else
            {
                // Create new translation
                var newTranslation = DishTranslation.Create(
                    request.Id,
                    translationDto.LanguageId,
                    translationDto.Name,
                    translationDto.Description);

                // Set Language navigation for mapping
                newTranslation.Language = languages.First(l => l.Id == translationDto.LanguageId);

                _translationRepository.Add(newTranslation);
            }
        }
        return Result.Success();
    }
}