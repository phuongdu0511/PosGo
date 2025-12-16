using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Enumerations;
using static PosGo.Contract.Services.V1.Account.Response;

namespace PosGo.Contract.Services.V1.Account;

public static class Query
{
    public record GetAccountMeQuery() : IQuery<AccountResponse>;
}
