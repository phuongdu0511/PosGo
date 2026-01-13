using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Language;
using PosGo.Domain.Abstractions.Repositories;

namespace PosGo.Application.UserCases.V1.Queries.Language;

public sealed class GetLanguagesQueryHandler : IQueryHandler<Query.GetLanguagesQuery, PagedResult<Response.LanguageResponse>>
{
    private readonly IRepositoryBase<Domain.Entities.Language, int> _languageRepository;
    private readonly IMapper _mapper;

    public GetLanguagesQueryHandler(
        IRepositoryBase<Domain.Entities.Language, int> languageRepository,
        IMapper mapper)
    {
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<Response.LanguageResponse>>> Handle(Query.GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Language> languagesQuery = _languageRepository.FindAll();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            languagesQuery = languagesQuery.Where(x => 
                x.Code.Contains(request.SearchTerm) || 
                x.Name.Contains(request.SearchTerm));
        }

        if (request.IsActive.HasValue)
        {
            languagesQuery = languagesQuery.Where(x => x.IsActive == request.IsActive.Value);
        }

        // Apply sorting
        if (request.SortColumnAndOrder?.Any() == true)
        {
            // Multi-column sorting - có thể implement raw SQL nếu cần
            languagesQuery = request.SortOrder == SortOrder.Descending
                ? languagesQuery.OrderByDescending(GetSortProperty(request))
                : languagesQuery.OrderBy(GetSortProperty(request));
        }
        else
        {
            languagesQuery = request.SortOrder == SortOrder.Descending
                ? languagesQuery.OrderByDescending(GetSortProperty(request))
                : languagesQuery.OrderBy(GetSortProperty(request));
        }

        var languages = await PagedResult<Domain.Entities.Language>.CreateAsync(
            languagesQuery, request.PageIndex, request.PageSize);
        
        var result = _mapper.Map<PagedResult<Response.LanguageResponse>>(languages);

        return Result.Success(result);
    }

    private static Expression<Func<Domain.Entities.Language, object>> GetSortProperty(Query.GetLanguagesQuery request)
        => request.SortColumn?.ToLower() switch
        {
            "code" => language => language.Code,
            "name" => language => language.Name,
            "isactive" => language => language.IsActive,
            "createdat" => language => language.CreatedAt,
            "updatedat" => language => language.UpdatedAt ?? language.CreatedAt,
            _ => language => language.CreatedAt // Default sort
        };
}