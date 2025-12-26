using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Account;
using PosGo.Domain.Entities;
using PosGo.Domain.Exceptions;
using PosGo.Domain.Utilities.Helpers;

namespace PosGo.Application.UserCases.V1.Commands.Account;

public sealed class UpdateProfileCommandHandler : ICommandHandler<Command.UpdateProfileCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProfileCommandHandler(
        UserManager<User> userManager,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result> Handle(Command.UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.GetCurrentUserId();
        var userUpdate = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new CommonNotFoundException.CommonException(userId, "Account");
        userUpdate.UpdateProfile(request.FullName, request.PhoneNumber);

        var result = _mapper.Map<Response.UpdateProfileResponse>(userUpdate);
        return Result.Success(result);
    }
}
