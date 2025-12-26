using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Entities;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Queries.Account;

public sealed class GetAccountMeQueryHandler : IQueryHandler<Query.GetAccountMeQuery, Response.AccountResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAccountMeQueryHandler(
        UserManager<User> userManager,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<Response.AccountResponse>> Handle(Query.GetAccountMeQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var account = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new CommonNotFoundException.CommonException(userId, "Account");

        var result = _mapper.Map<Response.AccountResponse>(account);

        return Result.Success(result);
    }
}
