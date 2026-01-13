using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Language;

public sealed class GetLanguageByCodeQueryHandler : IQueryHandler<Query.GetLanguageByCodeQuery, Response.LanguageResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public GetLanguageByCodeQueryHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result<Response.LanguageResponse>> Handle(Query.GetLanguageByCodeQuery request, CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToLowerInvariant();
        var language = await _languageRepository.FindSingleAsync(
            x => x.Code == normalizedCode, cancellationToken);
        
        if (language == null)
        {
            return Result.Failure<Response.LanguageResponse>(
                new Error("LANGUAGE_NOT_FOUND", "Không tìm thấy ngôn ngữ."));
        }

        var result = _mapper.Map<Response.LanguageResponse>(language);
        return Result.Success(result);
    }
}