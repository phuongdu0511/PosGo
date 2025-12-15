using Microsoft.AspNetCore.Identity;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Abstractions.Shared.CommonServices;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.Account;

public sealed class ChangePasswordUserCommandHandler : ICommandHandler<Command.ChangePasswordCommand>
{
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IPasswordHasher<Domain.Entities.User> _passwordHasher;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;
    public ChangePasswordUserCommandHandler(
        IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IPasswordHasher<Domain.Entities.User> passwordHasher,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(Command.ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is null)
        {
            return Result.Failure(new Error("UNAUTHORIZED", "User is not authenticated."));
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result.Failure(new Error(
                "PASSWORD_CONFIRM_NOT_MATCH",
                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
        }

        var userId = _currentUserService.UserId.Value;
        var user = await _userRepository.FindByIdAsync(userId) ?? throw new CommonNotFoundException.CommonException(userId, "Account");
        var verify = _passwordHasher.VerifyHashedPassword(user,user.Password,request.CurrentPassword);

        if (verify == PasswordVerificationResult.Failed)
        {
            return Result.Failure(new Error(
                "CURRENT_PASSWORD_INVALID",
                "Mật khẩu hiện tại không đúng."));
        }

        var newPasswordSame = _passwordHasher.VerifyHashedPassword(user, user.Password, request.NewPassword);

        if (newPasswordSame == PasswordVerificationResult.Success)
        {
            return Result.Failure(new Error(
                "PASSWORD_SAME_AS_OLD",
                "Mật khẩu mới không được trùng với mật khẩu hiện tại."));
        }

        var newHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.ChangePassword(newHash);

        await _cacheService.RemoveAsync(user.UserName, cancellationToken);

        return Result.Success();
    }
}
