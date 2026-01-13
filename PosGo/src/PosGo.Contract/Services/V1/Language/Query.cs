using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Language.Response;

namespace PosGo.Contract.Services.V1.Language;

public static class Query
{
    public record GetLanguagesQuery(
        string? SearchTerm,
        bool? IsActive,
        string? SortColumn, 
        SortOrder? SortOrder, 
        IDictionary<string, SortOrder>? SortColumnAndOrder, 
        int PageIndex, 
        int PageSize) : IQuery<PagedResult<LanguageResponse>>;
    
    public record GetLanguageByIdQuery(int Id) : IQuery<LanguageResponse>;
    
    public record GetLanguageByCodeQuery(string Code) : IQuery<LanguageResponse>;
    
    public record GetActiveLanguagesQuery() : IQuery<List<LanguageResponse>>;
}