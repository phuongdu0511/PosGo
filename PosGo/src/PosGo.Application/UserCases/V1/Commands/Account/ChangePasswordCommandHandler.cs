using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Entities;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Account;

public sealed class ChangePasswordUserCommandHandler : ICommandHandler<Command.ChangePasswordCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;
    public ChangePasswordUserCommandHandler(
        ICacheService cacheService, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _cacheService = cacheService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(Command.ChangePasswordCommand request, CancellationToken cancellationToken)
    {

        if (!string.Equals(request.NewPassword, request.ConfirmNewPassword, StringComparison.Ordinal))
        {
            return Result.Failure(new Error(
                "PASSWORD_CONFIRM_NOT_MATCH",
                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
        }

        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new CommonNotFoundException.CommonException(userId, "Account");
        var currentOk = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!currentOk)
        {
            return Result.Failure(new Error("CURRENT_PASSWORD_INVALID",
                "Mật khẩu hiện tại không đúng."));
        }

        var newSameAsCurrent = await _userManager.CheckPasswordAsync(user, request.NewPassword);
        if (newSameAsCurrent)
        {
            return Result.Failure(new Error("NEW_PASSWORD_SAME_AS_CURRENT",
                "Mật khẩu mới không được trùng với mật khẩu hiện tại."));
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
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
