using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.User;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.User;

public sealed class GetUserByIdQueryHandler : IQueryHandler<Query.GetUserByIdQuery, Response.UserResponse>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(UserManager<Domain.Entities.User> userManager,
        IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<Result<Response.UserResponse>> Handle(Query.GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString())
            ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

        var result = _mapper.Map<Response.UserResponse>(user);

        return Result.Success(result);
    }
}
