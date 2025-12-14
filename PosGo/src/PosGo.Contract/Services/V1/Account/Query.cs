using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Account.Response;

namespace PosGo.Contract.Services.V1.Account;

public static class Query
{
    public record GetAccountsQuery(string? SearchTerm, string? SortColumn, SortOrder? SortOrder, IDictionary<string, SortOrder>? SortColumnAndOrder, int PageIndex, int PageSize) : IQuery<PagedResult<AccountResponse>>;
    public record GetAccountByIdQuery(Guid Id) : IQuery<AccountResponse>;
    public record GetAccountMe() : IQuery<AccountResponse>;
}
