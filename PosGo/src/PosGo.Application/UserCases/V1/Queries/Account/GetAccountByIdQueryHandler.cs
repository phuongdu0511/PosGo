using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.account;

public sealed class GetAccountByIdQueryHandler : IQueryHandler<Query.GetAccountByIdQuery, Response.AccountResponse>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IMapper _mapper;

    public GetAccountByIdQueryHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<Result<Response.AccountResponse>> Handle(Query.GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _userRepository.FindByIdAsync(request.Id)
            ?? throw new AccountException.AccountNotFoundException(request.Id);

        var result = _mapper.Map<Response.AccountResponse>(account);

        return Result.Success(result);
    }
}
