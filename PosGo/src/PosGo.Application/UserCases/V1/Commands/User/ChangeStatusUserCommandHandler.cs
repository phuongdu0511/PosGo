using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Contract.Services.V1.User;
using Microsoft.AspNetCore.Identity;

namespace PosGo.Application.UserCases.V1.Commands.User;

public sealed class ChangeStatusUserCommandHandler : ICommandHandler<Command.ChangeStatusUserCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;

    public ChangeStatusUserCommandHandler(UserManager<Domain.Entities.User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result> Handle(Command.ChangeStatusUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString()) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));
        user.Status = request.status;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) {
            return Result.Failure(new Error("IDENTITY_UPDATE_FAILED", "Lỗi đổi trạng thái"));
        }
        return Result.Success();
    }
}
