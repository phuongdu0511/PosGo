using AutoMapper;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.User;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.User;

public sealed class GetUserByIdQueryHandler : IQueryHandler<Query.GetUserByIdQuery, Response.UserResponse>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<Result<Response.UserResponse>> Handle(Query.GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id)
            ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

        var result = _mapper.Map<Response.UserResponse>(user);

        return Result.Success(result);
    }
}
