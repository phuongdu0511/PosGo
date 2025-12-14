using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Contract.Services.V1.Account;
using PosGo.Contract.Abstractions.Shared.CommonServices;

namespace PosGo.Application.UserCases.V1.Commands.Account;

public sealed class UpdateProfileCommandHandler : ICommandHandler<Command.UpdateProfileCommand>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(IRepositoryBase<Domain.Entities.User, Guid> userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }
    public async Task<Result> Handle(Command.UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure<Response.AccountResponse>(
                new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        var userId = _currentUserService.UserId.Value;
        var userUpdate = await _userRepository.FindByIdAsync(userId) ?? throw new ProductException.ProductNotFoundException(userId);
        userUpdate.UpdateProfile(request.FullName, request.Phone);
        var result = new Response.UpdateProfileResponse(
            Id: userUpdate.Id,
            FullName: userUpdate.FullName ?? string.Empty,
            Phone: userUpdate.Phone ?? string.Empty
        );
        return Result.Success(result);
    }
}
