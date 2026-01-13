using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Language;

public sealed class GetActiveLanguagesQueryHandler : IQueryHandler<Query.GetActiveLanguagesQuery, List<Response.LanguageResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public GetActiveLanguagesQueryHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<Response.LanguageResponse>>> Handle(Query.GetActiveLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await _languageRepository.FindAll(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<Response.LanguageResponse>>(languages);
        return Result.Success(result);
    }
}