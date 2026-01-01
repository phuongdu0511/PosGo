using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.User;

namespace PosGo.Application.UserCases.V1.Commands.User;

public sealed class CreateUserCommandHandler : ICommandHandler<Command.CreateUserCommand>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(UserManager<Domain.Entities.User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<Result> Handle(Command.CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userExisted = await _userManager.FindByNameAsync(request.UserName);
        if (userExisted is not null)
        {
            return Result.Failure<Response.UserResponse>(
                new Error("EXISTED", "UserName đã tồn tại"));
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return Result.Failure(new Error(
                "PASSWORD_CONFIRM_NOT_MATCH",
                "Mật khẩu mới và xác nhận mật khẩu không trùng nhau."));
        }

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
        };

        var createdResult = await _userManager.CreateAsync(user, request.Password);
        if (!createdResult.Succeeded)
        {
            var codes = createdResult.Errors
                .Select(e => e.Code)
                .Distinct()
                .ToList();

            return Result.Failure(new Error(
                code: JsonSerializer.Serialize(codes),
                message: "VALIDATION_FAILED"
            ));
        }

        var result = _mapper.Map<Response.UserResponse>(user);

        return Result.Success(result);
    }
}
