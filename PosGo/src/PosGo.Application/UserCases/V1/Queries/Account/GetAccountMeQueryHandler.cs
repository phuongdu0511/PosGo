using AutoMapper;
using PosGo.Contract.Abstractions.Shared.CommonServices;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.account;

public sealed class GetAccountMeQueryHandler : IQueryHandler<Query.GetAccountMe, Response.AccountResponse>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetAccountMeQueryHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    public async Task<Result<Response.AccountResponse>> Handle(Query.GetAccountMe request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<Response.AccountResponse>(
                new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId.Value;

        var account = await _userRepository.FindByIdAsync(userId)
            ?? throw new AccountException.AccountNotFoundException(userId);

        var result = _mapper.Map<Response.AccountResponse>(account);

        return Result.Success(result);
    }
}
