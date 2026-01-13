using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Language;

public sealed class CreateLanguageCommandHandler : ICommandHandler<Command.CreateLanguageCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public CreateLanguageCommandHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra trùng Code
        var normalizedCode = request.Code.Trim().ToLowerInvariant();
        var existingLanguage = await _languageRepository.FindSingleAsync(
            x => x.Code == normalizedCode, cancellationToken);

        if (existingLanguage != null)
        {
            return Result.Failure(new Error("LANGUAGE_CODE_EXISTS", "Mã ngôn ngữ đã tồn tại."));
        }

        // Tạo Language mới
        var language = Domain.Entities.Language.Create(
            request.Code,
            request.Name,
            request.IsActive);

        _languageRepository.Add(language);

        var result = _mapper.Map<Response.LanguageResponse>(language);
        return Result.Success(result);
    }
}