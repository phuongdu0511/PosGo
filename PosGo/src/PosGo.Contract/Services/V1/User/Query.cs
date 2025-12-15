using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.User.Response;

namespace PosGo.Contract.Services.V1.User;

public static class Query
{
    public record GetUsersQuery(string? SearchTerm, string? SortColumn, SortOrder? SortOrder, IDictionary<string, SortOrder>? SortColumnAndOrder, int PageIndex, int PageSize) : IQuery<PagedResult<UserResponse>>;
    public record GetUserByIdQuery(Guid Id) : IQuery<UserResponse>;
    public record GetUsersByRoleQuery(string? SearchTerm, string? SortColumn, SortOrder? SortOrder, IDictionary<string, SortOrder>? SortColumnAndOrder, int PageIndex, int PageSize) : IQuery<PagedResult<UserResponse>>;
    public record GetUserByRoleQuery(Guid Id) : IQuery<UserResponse>;
}
