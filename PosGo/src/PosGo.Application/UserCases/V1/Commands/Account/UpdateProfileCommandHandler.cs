using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Contract.Services.V1.Account;
using PosGo.Contract.Abstractions.Shared.CommonServices;
using AutoMapper;

namespace PosGo.Application.UserCases.V1.Commands.Account;

public sealed class UpdateProfileCommandHandler : ICommandHandler<Command.UpdateProfileCommand>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository, ICurrentUserService currentUserService, IMapper mapper)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }
    public async Task<Result> Handle(Command.UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<Response.AccountResponse>(
                new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId.Value;
        var userUpdate = await _userRepository.FindByIdAsync(userId) ?? throw new CommonNotFoundException.CommonException(userId, "Account");
        userUpdate.UpdateProfile(request.FullName, request.Phone);

        var result = _mapper.Map<Response.UpdateProfileResponse>(userUpdate);
        return Result.Success(result);
    }
}
