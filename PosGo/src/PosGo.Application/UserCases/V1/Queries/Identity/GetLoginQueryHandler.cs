using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosGo.Application.Abstractions;
using PosGo.Contract.Abstractions.Shared;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Contract.Services.V1.Identity;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Persistence;

namespace PosGo.Application.UserCases.V1.Queries.Identity;

public class GetLoginQueryHandler : IQueryHandler<Query.Login, Response.Authenticated>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheService _cacheService;
    private readonly IRepositoryBase<Domain.Entities.User, Guid> _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public GetLoginQueryHandler(
        ApplicationDbContext dbContext,
        IJwtTokenService jwtTokenService, ICacheService cacheService,
        IRepositoryBase<Domain.Entities.User, Guid> userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _cacheService = cacheService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Response.Authenticated>> Handle(Query.Login request, CancellationToken cancellationToken)
    {
        // 1. Check User
        var user = await _userRepository.FindSingleAsync(x => x.UserName == request.UserName && x.Status == EUserStatus.Active, cancellationToken);

        if (user is null)
            return Result.Failure<Response.Authenticated>(new Error("INVALID_CREDENTIALS", "User not found."));

        // 2. Verify password
        var verify = _passwordHasher.VerifyHashedPassword(
            user,
            user.Password,
            request.Password);

        if (verify == PasswordVerificationResult.Failed)
        {
            return Result.Failure<Response.Authenticated>(new Error("INVALID_CREDENTIALS", "User not found."));
        }

        var roleCodes = await (
                from usr in _dbContext.UserSystemRoles
                join r in _dbContext.Roles on usr.RoleId equals r.Id
                where usr.UserId == user.Id && r.Scope == SystemConstants.Scope.SYSTEM && r.IsActive
                select r.Code
            ).ToListAsync(cancellationToken);

        // Generate JWT Token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        foreach (var rc in roleCodes)
        {
            claims.Add(new Claim(ClaimTypes.Role, rc));
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(claims);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new Response.Authenticated()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(5)
        };

        await _cacheService.SetAsync(request.UserName, response);

        return Result.Success(response);
    }
}
