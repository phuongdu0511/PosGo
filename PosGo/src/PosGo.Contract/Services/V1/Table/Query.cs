using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Table.Response;

namespace PosGo.Contract.Services.V1.Table;

public static class Query
{
    // Table Queries
    public record GetTablesByAreaQuery(Guid AreaId) : IQuery<List<TableResponse>>;
    
    public record GetTablesByRestaurantQuery(
        string? SearchTerm, 
        Guid? AreaId, 
        bool? IsActive,
        string? SortColumn, 
        SortOrder? SortOrder, 
        IDictionary<string, SortOrder>? SortColumnAndOrder, 
        int PageIndex, 
        int PageSize) : IQuery<PagedResult<TableResponse>>;
    
    public record GetTableByQrCodeQuery(string QrCodeToken) : IQuery<TableResponse>;
}