using PosGo.Contract.Abstractions.Shared;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Exceptions;
using PosGo.Contract.Services.V1.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace PosGo.Application.UserCases.V1.Commands.User;

public sealed class UpdateUserCommandHandler : ICommandHandler<Command.UpdateUserCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(UserManager<Domain.Entities.User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<Result> Handle(Command.UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString()) ?? throw new CommonNotFoundException.CommonException(request.Id, nameof(User));

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var codes = string.Join(",", updateResult.Errors.Select(e => e.Code));
            return Result.Failure(new Error("IDENTITY_UPDATE_FAILED", codes));
        }

        var result = _mapper.Map<Response.UpdateUserResponse>(user);
        return Result.Success(result);
    }
}
