using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Commands.Language;

public sealed class UpdateLanguageStatusCommandHandler : ICommandHandler<Command.UpdateLanguageStatusCommand>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public UpdateLanguageStatusCommandHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.UpdateLanguageStatusCommand request, CancellationToken cancellationToken)
    {
        // Tìm Language
        var language = await _languageRepository.FindByIdAsync(request.Id, cancellationToken);
        if (language == null)
        {
            return Result.Failure(new Error("LANGUAGE_NOT_FOUND", "Không tìm thấy ngôn ngữ."));
        }

        // Cập nhật trạng thái
        if (request.IsActive)
            language.Activate();
        else
            language.Deactivate();

        var result = _mapper.Map<Response.LanguageResponse>(language);
        return Result.Success(result);
    }
}