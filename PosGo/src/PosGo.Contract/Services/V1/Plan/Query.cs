using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Plan.Response;

namespace PosGo.Contract.Services.V1.Plan;

public static class Query
{
    public record GetPlansQuery(string? SearchTerm, string? SortColumn, SortOrder? SortOrder, IDictionary<string, SortOrder>? SortColumnAndOrder, int PageIndex, int PageSize) : IQuery<PagedResult<PlanResponse>>;
    
    public record GetPlanByIdQuery(Guid Id) : IQuery<PlanResponse>;
    
    public record GetPlanFunctionsQuery(Guid PlanId) : IQuery<List<PlanFunctionResponse>>;
}
