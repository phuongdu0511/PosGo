using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Language;

public sealed class UpdateLanguageCommandHandler : ICommandHandler<Command.UpdateLanguageCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public UpdateLanguageCommandHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        // Tìm Language
        var language = await _languageRepository.FindByIdAsync(request.Id, cancellationToken);
        if (language == null)
        {
            return Result.Failure(new Error("LANGUAGE_NOT_FOUND", "Không tìm thấy ngôn ngữ."));
        }

        // Kiểm tra trùng Code (nếu thay đổi)
        var normalizedCode = request.Code.Trim().ToLowerInvariant();
        if (language.Code != normalizedCode)
        {
            var existingLanguage = await _languageRepository.FindSingleAsync(
                x => x.Code == normalizedCode, cancellationToken);

            if (existingLanguage != null)
            {
                return Result.Failure(new Error("LANGUAGE_CODE_EXISTS", "Mã ngôn ngữ đã tồn tại."));
            }
        }

        // Cập nhật Language
        language.Update(request.Code, request.Name, request.IsActive);

        var result = _mapper.Map<Response.LanguageResponse>(language);
        return Result.Success(result);
    }
}