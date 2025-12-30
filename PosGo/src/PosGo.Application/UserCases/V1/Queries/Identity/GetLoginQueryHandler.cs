using Microsoft.AspNetCore.Identity;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Services.V1.Identity;
using PosGo.Domain.Exceptions;

namespace PosGo.Application.UserCases.V1.Queries.Identity;

public class GetLoginQueryHandler : IQueryHandler<Query.Login, Response.Authenticated>
{
    private readonly UserManager<Domain.Entities.User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;

    public GetLoginQueryHandler(UserManager<Domain.Entities.User> userManager,
        IJwtTokenService jwtTokenService, ICacheService cacheService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.Login request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null)
            throw new UserException.UserNotFoundException(request.UserName);


        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!checkPassword)
        {
            await _userManager.AccessFailedAsync(user);
            throw new UserException.UserPasswordException();
        }

        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var result = new Response.Authenticated()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(5)
        };

        await _cacheService.SetAsync(request.UserName, result);

        return Result.Success(result);
    }
}
