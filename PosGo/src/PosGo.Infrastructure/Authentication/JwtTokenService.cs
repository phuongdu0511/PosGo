using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PosGo.Application.Abstractions;
using PosGo.Contract.Common.Constants;
using PosGo.Contract.Enumerations;
using PosGo.Domain.Abstractions.Repositories;
using PosGo.Domain.Entities;
using PosGo.Domain.Utilities.Helpers;
using PosGo.Infrastructure.DependencyInjection.Options;
using PosGo.Persistence;

namespace PosGo.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOption jwtOption = new JwtOption();
    private readonly UserManager<User> _userManager;
    private readonly IRepositoryBase<Domain.Entities.RestaurantUser, int> _restaurantUserRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPermissionService _permissionService;

    public JwtTokenService(
        IConfiguration configuration,
        UserManager<User> userManager,
        IRepositoryBase<RestaurantUser, int> restaurantUserRepository,
        ApplicationDbContext dbContext,
        IPermissionService permissionService)
    {
        configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);
        _userManager = userManager;
        _restaurantUserRepository = restaurantUserRepository;
        _dbContext = dbContext;
        _permissionService = permissionService;
    }
    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        // 1. Lấy các mapping RestaurantUser đang Active của user
        var restaurantUsers = await _restaurantUserRepository
            .FindAll(x => x.UserId == user.Id && x.Status == ERestaurantUserStatus.Active)
            .ToListAsync();

        var restaurantIds = restaurantUsers
            .Select(x => x.RestaurantId)
            .Distinct()
            .ToList();

        // 2. Nếu user chỉ thuộc đúng 1 nhà hàng -> auto set RestaurantId
        Guid? activeRestaurantId = null;
        if (restaurantIds.Count == 1)
        {
            activeRestaurantId = restaurantIds[0];
        }

        var roleNames = await _userManager.GetRolesAsync(user);

        var isSystemUser = await _dbContext.Roles.AnyAsync(r =>
            roleNames.Contains(r.Name) &&
            r.Scope == SystemConstants.Scope.SYSTEM);

        var scope = isSystemUser ? SystemConstants.Scope.SYSTEM : SystemConstants.Scope.RESTAURANT;

        var ActionDes = EnumHelper<ActionType>.GetNameAndDescription().Values;
        var roles = await _permissionService.GetUserPermissionsAsync(user, scope, activeRestaurantId);

        var tokenKey = jwtOption.SecretKey;
        var issuer = jwtOption.Issuer;
        var audience = jwtOption.Audience;
        var expire = jwtOption.ExpireMin;

        var dateExpire = DateTime.UtcNow.AddHours(7).AddMinutes(expire);
        var claims = new List<Claim>
        {
            new Claim(SystemConstants.ClaimTypes.SCOPE, scope),
            new Claim(ClaimTypes.GivenName,user.FullName),
            new Claim(nameof(ActionDes),JsonConvert.SerializeObject(ActionDes)),
            new Claim(ClaimTypes.Role,JsonConvert.SerializeObject(roles)),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        // 3. Nếu xác định được 1 RestaurantId duy nhất => add vào token
        if (activeRestaurantId.HasValue)
        {
            claims.Add(new Claim(SystemConstants.ClaimTypes.RESTAURANT_ID, activeRestaurantId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: dateExpire, signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateAccessTokenForRestaurantAsync(User user, string scope, Guid restaurantId)
    {
        var ActionDes = EnumHelper<ActionType>.GetNameAndDescription().Values;
        var roles = await _permissionService.GetUserPermissionsAsync(user, scope, restaurantId);

        var tokenKey = jwtOption.SecretKey;
        var issuer = jwtOption.Issuer;
        var audience = jwtOption.Audience;
        var expire = jwtOption.ExpireMin;

        var dateExpire = DateTime.UtcNow.AddHours(7).AddMinutes(expire);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, user.FullName ?? string.Empty),
            new Claim(nameof(ActionDes), JsonConvert.SerializeObject(ActionDes)),
            new Claim(ClaimTypes.Role, JsonConvert.SerializeObject(roles)),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(SystemConstants.ClaimTypes.RESTAURANT_ID, restaurantId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: dateExpire,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var Key = Encoding.UTF8.GetBytes(jwtOption.SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Key),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

}
