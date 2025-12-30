using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.User;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Commands.User;

public sealed class ChangePasswordUserCommandHandler : ICommandHandler<Command.ChangePasswordUserCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly ICacheService _cacheService;
    public ChangePasswordUserCommandHandler(
        UserManager<Domain.Entities.User> userManager,
        ICacheService cacheService)
    {
        _userManager = userManager;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(Command.ChangePasswordUserCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
        {
            return Result.Failure(new Error(
                "PASSWORD_CONFIRM_NOT_MATCH",
                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
        }

        var user = await _userManager.FindByIdAsync(request.Id.ToString()) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            var codes = result.Errors
                .Select(e => e.Code)
                .Distinct()
                .ToList();

            return Result.Failure(new Error(
                code: JsonSerializer.Serialize(codes),
                message: "VALIDATION_FAILED"
            ));
        }

        await _cacheService.RemoveAsync(user.UserName!, cancellationToken);

        return Result.Success();
    }
}
