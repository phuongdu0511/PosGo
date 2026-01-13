using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Language;

public sealed class DeleteLanguageCommandHandler : ICommandHandler<Command.DeleteLanguageCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;

    public DeleteLanguageCommandHandler(IRepositoryBase<Domain.Entities.Language, int> languageRepository)
    {
        _languageRepository = languageRepository;
    }

    public async Task<Result> Handle(Command.DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        // Tìm Language
        var language = await _languageRepository.FindByIdAsync(request.Id, cancellationToken);
        if (language == null)
        {
            return Result.Failure(new Error("LANGUAGE_NOT_FOUND", "Không tìm thấy ngôn ngữ."));
        }

        // Kiểm tra có đang được sử dụng không
        var hasTranslations = await _languageRepository.FindAll()
            .Where(l => l.Id == request.Id)
            .SelectMany(l => l.CodeItemTranslations.Concat<object>(l.UnitTranslations)
                .Concat(l.DishCategoryTranslations)
                .Concat(l.DishTranslations)
                .Concat(l.DishVariantTranslations)
                .Concat(l.DishVariantOptionTranslations))
            .AnyAsync(cancellationToken);

        if (hasTranslations)
        {
            return Result.Failure(new Error("LANGUAGE_IN_USE", "Không thể xóa ngôn ngữ đang được sử dụng."));
        }

        // Xóa Language
        _languageRepository.Remove(language);

        return Result.Success();
    }
}